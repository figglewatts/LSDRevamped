using System;
using System.Collections;
using LSDR.Game;
using UnityEngine;

namespace LSDR.UI.Title
{
    public class UITitleScreenInitialisation : MonoBehaviour
    {
        public GameObject LoadingIcon;
        public GameObject Background;
        public GameObject MainMenu;
        
        public void Start()
        {
            StartCoroutine(LoadGameResources());
        }

        public IEnumerator LoadGameResources()
        {
            yield return StartCoroutine(GameScript.InitializeGame());
            LoadingIcon.SetActive(false);
            Background.SetActive(false);
            MainMenu.SetActive(true);
            Fader.FadeOut(Color.black, 5, () => gameObject.SetActive(false));
            yield return null;
        }
    }
}