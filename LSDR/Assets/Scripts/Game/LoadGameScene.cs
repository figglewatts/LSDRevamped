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
        public void Start()
        {
            Coroutines.Instance.StartCoroutine(loadTitle());
        }

        protected IEnumerator loadTitle()
        {
            AsyncOperation loadTitleScene = SceneManager.LoadSceneAsync("titlescreen", LoadSceneMode.Additive);
            yield return loadTitleScene;
            ToriiFader.Instance.SetFade(Color.black);
            Destroy(gameObject);
        }
    }
}
