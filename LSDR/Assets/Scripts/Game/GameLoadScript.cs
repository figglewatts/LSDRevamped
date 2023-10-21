using LSDR.Audio;
using LSDR.Dream;
using LSDR.InputManagement;
using LSDR.Lua;
using LSDR.SDK.Audio;
using LSDR.SDK.Data;
using LSDR.SDK.DreamControl;
using LSDR.SDK.Entities;
using LSDR.SDK.Lua;
using LSDR.SDK.Visual;
using LSDR.Visual;
using Torii.Console;
using Torii.Event;
using Torii.UI;
using UnityEngine;

namespace LSDR.Game
{
    public class GameLoadScript : MonoBehaviour
    {
        public GameLoadSystem GameLoadSystem;
        public DreamSystem DreamSystem;
        public ToriiEvent OnGameLaunch;
        public SettingsSystem SettingsSystem;

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
                GameLoadSystem.OnGameLoadedProgrammatic += () =>
                {
                    Debug.Log("Initialising entities...");
                    EntityIndex.Instance.AllRegistered();
                };

                DevConsole.Initialise();
                DreamSystem.Initialise();

                // hook up interfaces to SDK
                LuaManager.ProvideManaged(new LuaEngine(DreamSystem, SettingsSystem));
                DreamControlManager.ProvideManaged(DreamSystem);
                FadeManager.ProvideManaged(ToriiFader.Instance);
                MixerGroupProviderManager.ProvideManaged(new MixerGroupProvider());

                SettingsSystem.ModLoaderSystem.LoadMods();

                Screenshotter.Instance.Initialise();

                // set the sort order for the fader so the version text appears on top during fades
                ToriiFader.Instance.SetSortOrder(idx: 0);

                GameLoadSystem.ControlSchemeLoaderSystem.LoadSchemes();
                SettingsSystem.Load();
                GameLoadSystem.GameSaveSystem.Load();

                var player = GameObject.FindWithTag("Player");
                EntityIndex.Instance.Register("__player", player, force: true);
                DreamSystem.Player = player;

                GameObject camera = GameObject.FindWithTag("MainCamera");
                if (camera == null) Debug.LogWarning("Unable to find MainCamera in scene");
                EntityIndex.Instance.Register("__camera", camera, force: true);
                Debug.Log("finished test load");

                EntityIndex.Instance.AllRegistered();
            }
        }

        public void LoadGame()
        {
            #if UNITY_EDITOR
            // make sure we don't really slow down the editor when loading a lot of data
            Application.backgroundLoadingPriority = ThreadPriority.Low;
            #endif

            StartCoroutine(GameLoadSystem.LoadGameCoroutine(Testing));
        }
    }
}
