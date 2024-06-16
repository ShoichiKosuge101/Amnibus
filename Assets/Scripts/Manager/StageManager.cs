using Enemy;
using Map;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using SceneUtility = Utils.SceneUtility;

namespace Manager
{
    /// <summary>
    /// ステージマネージャ
    /// </summary>
    public class StageManager
        : MonoBehaviour
    {
        /// <summary>
        /// ステージデータ
        /// </summary>
        public StageData stageData;
        
        public float tileSize = 1.0f;
        
        private Camera _mainCamera;
        
        /// <summary>
        /// ゴールの位置
        /// </summary>
        private Vector3 _goalPosition;
        
        /// <summary>
        /// 開始
        /// </summary>
        private void Start()
        {
            _mainCamera = Camera.main;
            if(_mainCamera == null)
            {
                UnityEngine.Debug.LogError("Main Camera is missing");
                return;
            }
            
            Assert.IsNotNull(stageData);
            
            // ステージの初期化
            InitializeStage();
            
            // カメラの位置を調整
            AdjustCameraPosition();
        }

        /// <summary>
        /// カメラの位置を調整
        /// </summary>
        private void AdjustCameraPosition()
        {
            // ステージの中心を計算
            float stageCenterX = stageData.width * tileSize / 2;
            float stageCenterY = stageData.height * tileSize / 2;
            Vector3 stageCenter = new Vector3(stageCenterX, stageCenterY, -10f);
            
            // カメラの位置をステージの中心に設定
            _mainCamera.transform.position = new Vector3(
                stageCenter.x, 
                stageCenter.y, 
                _mainCamera.transform.position.z);
            
            // カメラのサイズをステージの大きさに合わせる
            float aspectRatio = _mainCamera.aspect;
            float verticalSize = stageData.height * tileSize / 2;
            float horizontalSize = stageData.width * tileSize / (2 * aspectRatio);

            // 縦横両方のサイズを比較して大きい方を採用
            const float padding = 1.0f;
            _mainCamera.orthographicSize = Mathf.Max(verticalSize, horizontalSize) * padding;
        }

        /// <summary>
        /// ステージの初期化
        /// </summary>
        private void InitializeStage()
        {
            // GridManagerの初期化
            GridManager.Instance.Initialize();

            // ゴールの生成
            GenerateGoal();

            // Map生成
            GenerateMap();
            
            // プレイヤーの生成
            GeneratePlayer();
            
            // 敵の生成
            GenerateEnemies();
        }

        /// <summary>
        /// ゴールの生成
        /// </summary>
        private void GenerateGoal()
        {
            int goalX, goalY;
            do
            {
                goalX = Random.Range(1, stageData.width - 1);
                goalY = Random.Range(1, stageData.height - 1);
            } while (GridManager.Instance.IsWall(goalX, goalY) || IsMapEdge(goalX, goalY));
            
            _goalPosition = new Vector3(goalX * tileSize, goalY * tileSize, 0);
            
            // ゴールを加算シーンに属する様に生成
            var goalScene = SceneManager.GetSceneByName(SceneUtility.GetScenePath(GameManager.Instance.GetCurrentScene()));
            var goalInstance = Instantiate(stageData.goalPrefab, _goalPosition, Quaternion.identity);
            SceneManager.MoveGameObjectToScene(goalInstance, goalScene);
            
            // ゴールの位置を設定
            GameManager.Instance.SetGoalPosition(_goalPosition);
        }

        /// <summary>
        /// マップ生成
        /// </summary>
        public void GenerateMap()
        {
            for (int i = 0; i < stageData.width; i++)
            {
                for (int j = 0; j < stageData.height; j++)
                {
                    GameObject tileObj;
                    // ゴールの位置に壁を生成しない
                    if (new Vector3(i * tileSize, j * tileSize, 0) == _goalPosition)
                    {
                        tileObj = stageData.floorPrefab;
                    }
                    else
                    {
                        // マップの端なら壁、それ以外なら床か壁をランダムで生成
                        tileObj = IsMapEdge(i, j) 
                            ? stageData.wallPrefab 
                            : GetRandomTile();
                    }
                    
                    Vector3 position = new Vector3(i * tileSize, j * tileSize,0);
                    var tile = Instantiate(tileObj, position, Quaternion.identity);
                    tile.transform.SetParent(transform);
                    
                    // GridManagerに壁の位置を登録
                    if (tileObj == stageData.wallPrefab)
                    {
                        GridManager.Instance.PlaceWall(i, j);
                    }
                }
            }
        }

        /// <summary>
        /// マップの端かどうか
        /// </summary>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <returns></returns>
        private bool IsMapEdge(int posX, int posY)
        {
            // マップの端かどうか
            return posX == 0 
                   || posY == 0 
                   || posX == stageData.width - 1 
                   || posY == stageData.height - 1;
        }

        /// <summary>
        /// プレイヤーの生成
        /// </summary>
        private void GeneratePlayer()
        {
            // ここでは固定位置に生成
            float playerX = (float)stageData.width / 2;
            float playerY = (float)stageData.height / 2;
            Vector3 spawnPosition = new Vector3(playerX, playerY, 0);
            
            // プレイヤーの位置が壁とゴールに被らないようにする
            while (GridManager.Instance.IsWall((int)playerX, (int)playerY)
                   || spawnPosition == _goalPosition)
            {
                playerX++;
                // ステージの端に到達したら反対側に移動
                if(playerX >= stageData.width - 1)
                {
                    playerX = 1;
                }
                
                playerY++;
                // ステージの端に到達したら反対側に移動
                if(playerY >= stageData.height - 1)
                {
                    playerY = 1;
                }
                
                spawnPosition = new Vector3(playerX * tileSize, playerY * tileSize, 0);
            }
            
            // プレイヤーを加算シーンに属する様に生成
            var playerScene = SceneManager.GetSceneByName(SceneUtility.GetScenePath(GameManager.Instance.GetCurrentScene()));
            var playerObj = Instantiate(GameManager.Instance.playerPrefab, spawnPosition, Quaternion.identity);
            SceneManager.MoveGameObjectToScene(playerObj, playerScene);
            
            // プレイヤーの設定
            GameManager.Instance.SetPlayerInstance(playerObj);
        }

        /// <summary>
        /// 敵の生成
        /// </summary>
        private void GenerateEnemies()
        {
            foreach (var enemyData in stageData.enemies)
            {
                int enemyCount = Random.Range(1, 5);
                for(int enemyIndex = 0; enemyIndex < enemyCount; enemyIndex++)
                {
                    // 敵の位置をランダムで決める。壁とマップ端と、ゴール上と、プレイヤーの上を避けること
                    Vector3 enemyPosition = GetRandomEnemySpawnPosition();
                    
                    // 敵を加算シーンに属する様に生成
                    var enemyScene = SceneManager.GetSceneByName(SceneUtility.GetScenePath(GameManager.Instance.GetCurrentScene()));
                    var enemyInstance = Instantiate(enemyData.enemyPrefab, enemyPosition, Quaternion.identity);
                    SceneManager.MoveGameObjectToScene(enemyInstance, enemyScene);
                    
                    // 敵の設定
                    var enemy = enemyInstance.GetComponent<EnemyBase>();
                    if (enemy != null)
                    {
                        enemy.Initialize(enemyData);
                    }
                }
            }
        }
        
        /// <summary>
        /// 生成位置を壁を避けて、ゴールとプレイヤーを避けてランダムで取得
        /// </summary>
        /// <returns></returns>
        private Vector3 GetRandomEnemySpawnPosition()
        {
            Vector3 playerPosition = GameManager.Instance.GetPlayerInstance().transform.position;
            // 敵の位置をランダムで決める。壁とマップ端と、ゴール上と、プレイヤーの上を避けること
            Vector3 enemyPosition;
            // 壁の位置を避けたランダムな位置を取得
            do 
            {
                enemyPosition = GetRandomPosition();
            } while (enemyPosition == _goalPosition 
                     || enemyPosition == playerPosition);
            
            return enemyPosition;
        }
        
        /// <summary>
        /// ランダムな位置を取得
        /// </summary>
        /// <returns></returns>
        private Vector3 GetRandomPosition()
        {
            // 壁に被らないようにランダムな位置を取得
            Vector3 randomPosition;
            do
            {
                randomPosition = GetRandomVector3();
            } while (GridManager.Instance.IsWall((int)randomPosition.x, (int)randomPosition.y));
            
            return randomPosition;
        }
        
        /// <summary>
        /// ランダムなVector3を取得
        /// </summary>
        /// <returns></returns>
        private Vector3 GetRandomVector3()
        {
            return new Vector3(
                Random.Range(1, stageData.width - 1) * tileSize, 
                Random.Range(1, stageData.height - 1) * tileSize, 
                0);
        }
        
        /// <summary>
        /// タイルのプレハブを判定
        /// </summary>
        /// <returns></returns>
        private GameObject GetRandomTile()
        {
            const float threshold = 0.8f;
            
            return Random.value > threshold
                ? stageData.wallPrefab 
                : stageData.floorPrefab;
        }
    }
}