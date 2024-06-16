using Base;
using UniRx;
using UnityEngine;

namespace Common.MainGame
{
    /// <summary>
    /// メインゲームシーン
    /// </summary>
    public class MainGameScene
        : SceneBase
    {
        /// <summary>
        /// プレゼンター
        /// </summary>
        [SerializeField]
        private MainGameScenePresenter presenter;

        /// <summary>
        /// シーン初期化
        /// </summary>
        protected override void InitializeScene()
        {
            presenter.Initialize();
            presenter.OnRequestSceneChangeRx
                .TakeUntilDestroy(this)
                .Subscribe(ChangeScene);
        }
    }
}