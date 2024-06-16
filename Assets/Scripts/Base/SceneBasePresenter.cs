using System;
using Constants;
using UniRx;
using UnityEngine;

namespace Base
{
    /// <summary>
    /// シーンクラス用プレゼンターベース
    /// </summary>
    public class SceneBasePresenter
        : MonoBehaviour
    {
        private readonly Subject<SceneName> _onRequestSceneChangeRx = new Subject<SceneName>();
        public IObservable<SceneName> OnRequestSceneChangeRx => _onRequestSceneChangeRx;
        
        /// <summary>
        /// シーン遷移リクエスト
        /// </summary>
        /// <param name="sceneName"></param>
        protected void RequestChangeScene(SceneName sceneName)
        {
            _onRequestSceneChangeRx.OnNext(sceneName);
        }
        
        /// <summary>
        /// エラーハンドリング
        /// </summary>
        /// <param name="exception"></param>
        public void HandleError(Exception exception)
        {
            // エラー処理
            UnityEngine.Debug.LogError("Error Occurred: " + exception.Message);
        }
    }
}