using System;
using System.Collections;
using Torii.Coroutine;
using Torii.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LSDR.Game
{
    public class LoadGameScene : MonoBehaviour
    {
        public void LoadIntro()
        {
            SceneManager.LoadScene("intro");
        }
    }
}
