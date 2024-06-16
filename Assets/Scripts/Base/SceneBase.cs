using System;
using Constants;
using UnityEngine;
using Utils;

namespace Base
{
    /// <summary>
    /// シーン基底クラス
    /// </summary>
    public class SceneBase
        : MonoBehaviour
    {
        /// <summary>
        /// 初期化
        /// </summary>
        private void Awake()
        {
            EnsureSceneNavigator();
        }

        /// <summary>
        /// シーンナビゲーターが存在するか確認
        /// </summary>
        /// <exception cref="Exception"></exception>
        private static void EnsureSceneNavigator()
        {
            if (SceneNavigator.Instance != null)
            {
                return;
            }
            
            var sceneNavigator = Resources.Load<SceneNavigator>("SceneNavigator");
            if (sceneNavigator == null)
            {
                throw new Exception("SceneNavigator is not found.");
            }
            // 生成
            Instantiate(sceneNavigator);
        }

        /// <summary>
        /// シーン初期化
        /// </summary>
        private void Start()
        {
            InitializeScene();
        }

        /// <summary>
        /// シーン初期化
        /// </summary>
        protected virtual void InitializeScene()
        {
            UnityEngine.Debug.Log("InitializeScene" + GetType().Name);
        }
        
        /// <summary>
        /// シーン遷移
        /// </summary>
        /// <param name="sceneName"></param>
        protected static void ChangeScene(SceneName sceneName)
        {
            SceneNavigator.Instance.LoadScene(SceneUtility.GetScenePath(sceneName));
        }
        
        /// <summary>
        /// シーンクリーンアップ
        /// </summary>
        protected virtual void CleanupScene()
        {
            // ここに共通のクリーンアップロジックを記述
            UnityEngine.Debug.Log("CleanupScene" + GetType().Name);
        }
    }
}