using Base;
using Constants;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Common.MainGame
{
    /// <summary>
    /// メインゲームシーンプレゼンター
    /// </summary>
    public class MainGameScenePresenter
        : SceneBasePresenter
    {
        /// <summary>
        /// タイトルボタン
        /// </summary>
        [SerializeField]
        private Button titleButton;
        
        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize()
        {
            // タイトルボタンをタップするとタイトルシーンに遷移する
            titleButton.OnClickAsObservable()
                .TakeUntilDestroy(this)
                .Subscribe(_ => 
                {
                    RequestChangeScene(SceneName.Title);
                });
        }
    }
}