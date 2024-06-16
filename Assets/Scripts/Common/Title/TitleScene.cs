using Base;
using UniRx;
using UnityEngine;

namespace Common.Title
{
    /// <summary>
    /// タイトルシーン
    /// </summary>
    public class TitleScene
        : SceneBase
    {
        /// <summary>
        /// プレゼンター
        /// </summary>
        [SerializeField]
        private TitleScenePresenter presenter;

        /// <summary>
        /// シーン初期化
        /// </summary>
        protected override void InitializeScene()
        {
            presenter.Initialize();
            
            // シーン遷移リクエストを受け取り、シーン遷移する
            presenter.OnRequestSceneChangeRx
                .TakeUntilDestroy(this)
                .Subscribe(ChangeScene);
        }
    }
}