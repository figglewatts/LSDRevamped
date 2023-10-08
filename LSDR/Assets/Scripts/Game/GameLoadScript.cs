using LSDR.Dream;
using LSDR.SDK.Entities;
using Torii.Event;
using UnityEngine;

namespace LSDR.Game
{
    public class GameLoadScript : MonoBehaviour
    {
        public GameLoadSystem GameLoadSystem;
        public DreamSystem DreamSystem;
        public ToriiEvent OnGameLaunch;

        public bool Testing = false;

        public void Start()
        {
            if (GameLoadSystem.GameLoaded)
            {
                Destroy(this.gameObject);
                return;
            }

            OnGameLaunch.Raise();

            if (Testing)
            {
                var player = GameObject.FindWithTag("Player");
                EntityIndex.Instance.Register("__player", player);
                DreamSystem.Player = player;

                GameObject camera = GameObject.FindWithTag("MainCamera");
                if (camera == null) Debug.LogWarning("Unable to find MainCamera in scene");
                EntityIndex.Instance.Register("__camera", camera);

                EntityIndex.Instance.AllRegistered();
            }
        }

        public void LoadGame()
        {
            #if UNITY_EDITOR
            // make sure we don't really slow down the editor when loading a lot of data
            Application.backgroundLoadingPriority = ThreadPriority.Low;
            #endif

            StartCoroutine(GameLoadSystem.LoadGameCoroutine());
        }
    }
}
