using LSDR.Game;
using UnityEngine;
using UnityEngine.UI;

namespace LSDR.UI.Title
{
    public class UIGameLoadError : MonoBehaviour
    {
        public GameObject ErrorDialog;
        public Button QuitButton;
        public GameObject LoadingView;

        public void OnGameLoadError()
        {
            LoadingView.SetActive(value: false);
            QuitButton.onClick.AddListener(Application.Quit);
            ErrorDialog.SetActive(value: true);
        }
    }
}
