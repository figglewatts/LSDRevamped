using LSDR.Game;
using UnityEngine;
using UnityEngine.UI;

namespace LSDR.UI.Title
{
    public class UIGameLoadError : MonoBehaviour
    {
        public GameLoadSystem GameLoadSystem;
        public GameObject ErrorDialog;
        public Text ErrorText;
        public Button QuitButton;
        public GameObject LoadingView;

        public void Start()
        {
            GameLoadSystem.OnGameLoadError += onGameLoadError;
        }

        protected void onGameLoadError(string error)
        {
            LoadingView.SetActive(false);
            ErrorText.text = error;
            QuitButton.onClick.AddListener(Application.Quit);
            ErrorDialog.SetActive(true);
        }
    }
}
