using System;
using System.Collections;
using Base;
using Constants;
using UnityEngine;
using UnityEngine.SceneManagement;
using SceneUtility = Utils.SceneUtility;

namespace Manager
{
    /// <summary>
    /// ゲームマネージャ
    /// </summary>
    public class GameManager
        : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        /// <summary>
        /// プレイヤーのプレハブ
        /// </summary>
        public GameObject playerPrefab;

        /// <summary>
        /// プレイヤーのインスタンス
        /// </summary>
        private GameObject _playerInstance;

        /// <summary>
        /// ゴールの位置
        /// </summary>
        private Vector3 _goalPosition;

        /// <summary>
        /// 現在のゲームの状態
        /// </summary>
        public MainGameStatus CurrentGameStatus { get; private set; } = MainGameStatus.Playing;
        public bool IsPlaying => CurrentGameStatus == MainGameStatus.Playing;
        
        /// <summary>
        /// 現在のシーン
        /// enumの値をステージの進行として扱う
        /// </summary>
        private SceneName _currentScene;

        /// <summary>
        /// 開始
        /// </summary>
        private void Awake()
        {
            // シングルトン
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 初期化
        /// </summary>
        private void Start()
        {
            InitializeGame();
            
            // GridManagerの初期化
            GridManager.Instance.Initialize();
        }

        /// <summary>
        /// ゲームの初期化
        /// </summary>
        private void InitializeGame()
        {
            // 最初のステージはStage1
            _currentScene = SceneName.Stage1;
            
            // ステージの加算シーンロード
            LoadCurrentStage();
        }

        /// <summary>
        /// 現在のステージをロード
        /// </summary>
        private void LoadCurrentStage()
        {
            string scenePath = SceneUtility.GetScenePath(_currentScene);
            SceneNavigator.Instance.LoadAdditiveScene(scenePath);
        }

        /// <summary>
        /// 次のステージをロード
        /// </summary>
        /// <param name="nextStage"></param>
        /// <returns></returns>
        private IEnumerator LoadNextStage(SceneName nextStage)
        {
            // 現在のステージをアンロード
            AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(SceneUtility.GetScenePath(_currentScene));
            while (unloadOperation == null 
                   || !unloadOperation.isDone)
            {
                yield return null;
            }
            
            // 次のステージをロード
            _currentScene = nextStage;
            AsyncOperation loadOperation = SceneManager.LoadSceneAsync(SceneUtility.GetScenePath(nextStage), LoadSceneMode.Additive);
            while (loadOperation == null 
                   || !loadOperation.isDone)
            {
                yield return null;
            }
            
            // 新しいシーンの初期化を待つ
            yield return new WaitForEndOfFrame();
        }

        /// <summary>
        /// ポーズ中かどうかを設定
        /// </summary>
        /// <param name="status"></param>
        public void SetGameState(MainGameStatus status)
        {
            CurrentGameStatus = status;
        }
        
        /// <summary>
        /// プレイヤーのインスタンスを設定
        /// </summary>
        /// <param name="playerInstance"></param>
        public void SetPlayerInstance(GameObject playerInstance)
        {
            _playerInstance = playerInstance;
        }
        
        /// <summary>
        /// プレイヤーのインスタンスを取得
        /// </summary>
        /// <returns></returns>
        public GameObject GetPlayerInstance()
        {
            return _playerInstance;
        }
        
        /// <summary>
        /// ゴールの位置を設定
        /// </summary>
        /// <param name="position"></param>
        public void SetGoalPosition(Vector3 position)
        {
            _goalPosition = position;
        }

        /// <summary>
        /// ゴールの位置を取得
        /// </summary>
        /// <returns></returns>
        public Vector3 GetGoalPosition()
        {
            return _goalPosition;
        }

        public void OnGoalReached()
        {
            if (HasNextStage())
            {
                var nextStage = GetNextStage();
                StartCoroutine(LoadNextStage(nextStage));
            }
            else
            {
                SetGameState(MainGameStatus.Win);
                
                // タイトルへ遷移
                GotoTitle();
            }
        }
        
        /// <summary>
        /// 次のステージが存在するかチェック
        /// </summary>
        /// <returns></returns>
        private bool HasNextStage()
        {
            var values = (SceneName[])Enum.GetValues(typeof(SceneName));
            int currentIndex = Array.IndexOf(values, _currentScene);
            return currentIndex< values.Length - 1;
        }
        
        /// <summary>
        /// 現在のステージを取得
        /// </summary>
        /// <returns></returns>
        public SceneName GetCurrentScene()
        {
            return _currentScene;
        }

        /// <summary>
        /// 次のステージを取得
        /// </summary>
        /// <returns></returns>
        private SceneName GetNextStage()
        {
            var values = (SceneName[])Enum.GetValues(typeof(SceneName));
            int currentIndex = Array.IndexOf(values, _currentScene);
            return values[currentIndex + 1];
        }

        /// <summary>
        /// タイトルへ遷移
        /// </summary>
        private void GotoTitle()
        {
            SceneNavigator.Instance.LoadScene(SceneUtility.GetScenePath(SceneName.Title));
        }

        /// <summary>
        /// クリーンアップ
        /// </summary>
        public void Cleanup()
        {
            // GridManagerのクリーンアップ
            GridManager.Instance.Cleanup();
            
            // シーンの破棄
            string scenePath = SceneUtility.GetScenePath(_currentScene);
            SceneNavigator.Instance.UnloadAdditiveScene(scenePath);
            
            // 自身のリセット
            Instance = null;
        }
    }
}