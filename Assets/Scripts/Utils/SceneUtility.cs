using Constants;

namespace Utils
{
    /// <summary>
    /// シーンパスを取得するユーティリティ
    /// </summary>
    public static class SceneUtility
    {
        /// <summary>
        /// シーン名からシーンパスを取得する
        /// </summary>
        /// <param name="sceneName"></param>
        /// <returns></returns>
        public static string GetScenePath(SceneName sceneName)
        {
            return sceneName switch
            {
                SceneName.Title    => "Scenes/Title",
                SceneName.MainGame => "Scenes/MainGame",
                SceneName.Stage1   => "Scenes/Stage1",
                SceneName.Stage2   => "Scenes/Stage2",
                SceneName.Stage3   => "Scenes/Stage3",
                _                  => null,
            };
        }
    }
}