using System;
using System.Collections;
using LSDR.Game;
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
            LoadingIcon.SetActive(false);
            Background.SetActive(false);
            MainMenu.SetActive(true);
            ToriiFader.Instance.FadeOut(Color.black, 5, null, 1);
        }
    }
}