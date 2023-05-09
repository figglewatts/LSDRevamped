using System;
using System.Collections;
using System.Reflection;
using LSDR.Dream;
using LSDR.InputManagement;
using LSDR.IO.ResourceHandlers;
using LSDR.Lua;
using LSDR.SDK.DreamControl;
using LSDR.SDK.Lua;
using LSDR.Visual;
using Torii.Console;
using Torii.Event;
using Torii.UI;
using UnityEngine;
using TResourceManager = Torii.Resource.ResourceManager; // TODO: change when old ResourceManager removed

[assembly: AssemblyVersion("0.2.*")]

namespace LSDR.Game
{
    /// <summary>
    ///     GameLoadSystem is a system that contains code for initialising the game.
    /// </summary>
    [CreateAssetMenu(menuName = "System/GameLoadSystem")]
    public class GameLoadSystem : ScriptableObject
    {
        [NonSerialized] public static bool GameLoaded;
        public ModLoaderSystem ModLoaderSystem;
        public ControlSchemeLoaderSystem ControlSchemeLoaderSystem;
        public SettingsSystem SettingsSystem;
        public GameSaveSystem GameSaveSystem;
        public DreamSystem DreamSystem;
        public ToriiEvent OnGameLoaded;
        public Action<string> OnGameLoadError;

        public void OnEnable()
        {
            OnGameLoadError += err => { Debug.LogError($"Game load error: {err}"); };
        }

        public IEnumerator LoadGameCoroutine()
        {
            Debug.Log("Loading game...");

            // do game startup stuff here
            DevConsole.Initialise();

            DreamSystem.Initialise();

            LuaManager.ProvideManaged(new LuaEngine());
            DreamControlManager.ProvideManaged(DreamSystem);

            TResourceManager.RegisterHandler(new LBDHandler());
            TResourceManager.RegisterHandler(new TIXHandler());
            TResourceManager.RegisterHandler(new Texture2DHandler());
            TResourceManager.RegisterHandler(new MOMHandler());
            TResourceManager.RegisterHandler(new ToriiAudioClipHandler());
            TResourceManager.RegisterHandler(new TIXTexture2DHandler());

            Screenshotter.Instance.Initialise();

            // set the sort order for the fader so the version text appears on top during fades
            ToriiFader.Instance.SetSortOrder(0);

            Shader.SetGlobalFloat("_FogStep", 0.08F);
            Shader.SetGlobalFloat("AffineIntensity", 0.5F);

            ControlSchemeLoaderSystem.LoadSchemes();
            SettingsSystem.Load();
            yield return ModLoaderSystem.LoadMods();
            if (!ModLoaderSystem.ModsAvailable)
            {
                OnGameLoadError?.Invoke("There are no mods available.\n\nPlease check your mod folder and try again.");
                yield break;
            }

            GameSaveSystem.Load();

            GameLoaded = true;
            OnGameLoaded.Raise();
        }
    }
}
