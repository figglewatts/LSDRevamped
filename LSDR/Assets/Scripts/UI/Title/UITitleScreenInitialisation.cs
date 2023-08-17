using Torii.UI;
using UnityEngine;

namespace LSDR.UI.Title
{
    public class UITitleScreenInitialisation : MonoBehaviour
    {
        public GameObject LoadingIcon;
        public GameObject Background;
        public GameObject MainMenu;

        public void ShowTitleScreen()
        {
            LoadingIcon.SetActive(value: false);
            Background.SetActive(value: false);
            MainMenu.SetActive(value: true);
            ToriiFader.Instance.FadeOut(Color.black, duration: 5, onFinish: null, initialAlpha: 1);
        }
    }
}
