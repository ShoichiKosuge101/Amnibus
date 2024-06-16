using Constants;
using Manager;
using UnityEngine;

namespace MainMenu
{
    /// <summary>
    /// メインメニューコントローラー
    /// </summary>
    public class MainMenuController
        : MonoBehaviour
    {
        public GameObject mainMenuPanel;
        
        private void Start()
        {
            // メニューは最初隠れている 
            mainMenuPanel.SetActive(false);
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Z))
            {
                ToggleMenu();
            }
        }

        private void ToggleMenu()
        {
            var currentGameStatus = GameManager.Instance.CurrentGameStatus == MainGameStatus.Playing
                ? MainGameStatus.Pause
                : MainGameStatus.Playing;
            
            GameManager.Instance.SetGameState(currentGameStatus);
            mainMenuPanel.SetActive(currentGameStatus == MainGameStatus.Pause);
        }
    }
}