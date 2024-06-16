using Manager;
using UnityEngine;

namespace Base
{
    /// <summary>
    /// シーン遷移クラス
    /// </summary>
    public class SceneNavigator
        : MonoBehaviour
    {
        /// <summary>
        /// インスタンス
        /// </summary>
        public static SceneNavigator Instance { get; private set; }
        
        /// <summary>
        /// 初期化
        /// </summary>
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                
                // シーン遷移時に破棄されないようにする
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(this);
            }
        }
        
        /// <summary>
        /// シーン遷移
        /// </summary>
        /// <param name="sceneName"></param>
        public void LoadScene(string sceneName)
        {
            // シーン遷移の前にシーンの破棄
            CleanupMainScene();
            
            // シーン遷移
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }
        
        /// <summary>
        /// 加算シーン遷移
        /// </summary>
        /// <param name="sceneName"></param>
        public void LoadAdditiveScene(string sceneName)
        {
            // シーン遷移
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                sceneName, 
                UnityEngine.SceneManagement.LoadSceneMode.Additive
                );
        }
        
        /// <summary>
        /// 加算シーンの破棄
        /// </summary>
        /// <param name="sceneName"></param>
        public void UnloadAdditiveScene(string sceneName)
        {
            // シーン遷移
            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(sceneName);
        }

        /// <summary>
        /// メインシーンの破棄
        /// </summary>
        private static void CleanupMainScene()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.Cleanup();
            }
        }
    }
}