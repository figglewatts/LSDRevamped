using System;
using System.Collections;
using System.Reflection;
using LSDR.InputManagement;
using LSDR.IO.ResourceHandlers;
using LSDR.Visual;
using Torii.UI;
using UnityEngine;
using TResourceManager = Torii.Resource.ResourceManager; // TODO: change when old ResourceManager removed

[assembly: AssemblyVersion("0.2.*")]

namespace LSDR.Game
{
    /// <summary>
    /// GameLoadSystem is a system that contains code for initialising the game.
    /// </summary>
    [CreateAssetMenu(menuName = "System/GameLoadSystem")]
    public class GameLoadSystem : ScriptableObject
    {
        public ModLoaderSystem ModLoaderSystem;
        public ControlSchemeLoaderSystem ControlSchemeLoaderSystem;
        public SettingsSystem SettingsSystem;
        public GameSaveSystem GameSaveSystem;

        [NonSerialized] public static bool GameLoaded = false;

        public IEnumerator LoadGameCoroutine()
        {
            // do game startup stuff here

            TResourceManager.RegisterHandler(new LBDHandler());
            TResourceManager.RegisterHandler(new TIXHandler());
            TResourceManager.RegisterHandler(new Texture2DHandler());
            TResourceManager.RegisterHandler(new MOMHandler());
            TResourceManager.RegisterHandler(new ToriiAudioClipHandler());
            TResourceManager.RegisterHandler(new TIXTexture2DHandler());

            Screenshotter.Instance.Init();

            // set the sort order for the fader so the version text appears on top during fades
            ToriiFader.Instance.SetSortOrder(0);

            Shader.SetGlobalFloat("_FogStep", 0.08F);
            Shader.SetGlobalFloat("AffineIntensity", 0.5F);

            ControlSchemeLoaderSystem.LoadSchemes();
            SettingsSystem.Load();
            yield return ModLoaderSystem.LoadMods();
            GameSaveSystem.Load();

            GameLoaded = true;
        }
    }
}
