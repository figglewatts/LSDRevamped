using System;
using System.IO;
using LSDR.Audio;
using LSDR.Dream;
using LSDR.InputManagement;
using LSDR.SDK.Data;
using LSDR.SDK.Visual;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Torii.Binding;
using Torii.Event;
using Torii.Serialization;
using Torii.Util;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.XR;

namespace LSDR.Game
{
    [CreateAssetMenu(menuName = "System/SettingsSystem")]
    public class SettingsSystem : ScriptableObject
    {
        /// <summary>
        ///     The framerate of the PS1. Used when framerate limiting is enabled.
        /// </summary>
        public const int FRAMERATE_LIMIT = 20;

        // reference to master audio mixer used for volume controls
        public AudioMixer MasterMixer;

        public ModLoaderSystem ModLoaderSystem;
        public ControlSchemeLoaderSystem ControlSchemeLoader;
        public MusicSystem MusicSystem;
        public DreamSystem DreamSystem;

        public ToriiEvent OnSettingsApply;
        public Action ProgrammaticOnSettingsApply;

        // reference to serializer used for loading/saving data
        private readonly ToriiSerializer _serializer = new ToriiSerializer();

        [NonSerialized] public readonly BindBroker SettingsBindBroker = new BindBroker();

        /// <summary>
        ///     Used to disable player motion, i.e. when linking.
        /// </summary>
        [NonSerialized] public bool CanControlPlayer = true;

        /// <summary>
        ///     Used to disable mouse looking, i.e. when paused.
        /// </summary>
        [NonSerialized] public bool CanMouseLook = true;

        /// <summary>
        ///     Used to disable player gravity, i.e. when initially spawning.
        /// </summary>
        [NonSerialized] public bool PlayerGravity = true;

        /// <summary>
        ///     Whether or not we're in VR mode.
        /// </summary>
        [NonSerialized] public bool VR;

        public GameSettings Settings { get; private set; }

        public LSDRevampedMod CurrentMod => ModLoaderSystem.GetMod(Settings.CurrentModIndex);
        public DreamJournal CurrentJournal => CurrentMod.GetJournal(Settings.CurrentJournalIndex);

        // the path to the settings serialized file
        private static string SettingsPath => PathUtil.Combine(Application.persistentDataPath, "settings.json");

        public void OnEnable()
        {
            VR = !XRSettings.loadedDeviceName.Equals(string.Empty);

            _serializer.RegisterJsonSerializationSettings(typeof(GameSettings), new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                SerializationBinder = new DefaultSerializationBinder()
            });
        }

        public void Load()
        {
            Debug.Log("Loading game settings...");

            // if we're loading over an existing settings object we want to deregister the old one
            if (Settings != null) SettingsBindBroker.DeregisterData(Settings);

            // check to see if the settings file exists
            if (File.Exists(SettingsPath))
            {
                Settings = _serializer.Deserialize<GameSettings>(SettingsPath);
            }
            else
            {
                // create the default settings
                Debug.Log("Settings.json not found, creating default settings");
                Settings = new GameSettings();
                Save();
            }

            if (Settings.Profiles.Count == 0) Settings.Profiles = SettingsProfile.CreateDefaultProfiles();

            // register the new settings object
            SettingsBindBroker.RegisterData(Settings);

            Settings.ApplyCurrentProfile();
            Apply();
        }

        public void Save()
        {
            Debug.Log("Saving game settings...");

            _serializer.Serialize(Settings, SettingsPath);
        }

        /// <summary>
        ///     Apply the game settings. This function propagates the given settings to all game systems that need them.
        /// </summary>
        public void Apply()
        {
            Debug.Log("Applying game settings");

            // set the control scheme
            ControlSchemeLoader.SelectScheme(Settings.CurrentControlSchemeIndex);

            // set the resolution
            if (Settings.CurrentResolutionIndex >= Screen.resolutions.Length || Settings.CurrentResolutionIndex < 0)
            {
                // if the resolution is invalid, set it to the native resolution
                Screen.SetResolution(Display.main.systemWidth, Display.main.systemHeight, Settings.Fullscreen);
            }
            else
            {
                Screen.SetResolution(Screen.resolutions[Settings.CurrentResolutionIndex].width,
                    Screen.resolutions[Settings.CurrentResolutionIndex].height, Settings.Fullscreen);
            }

            // set framerate to limit or not
            Application.targetFrameRate = Settings.LimitFramerate ? FRAMERATE_LIMIT : -1;

            // set retro shader affine intensity
            Shader.SetGlobalFloat("_AffineIntensity", Settings.AffineIntensity);

            // apply original soundtrack
            MusicSystem.UseOriginalSongs(Settings.UseOriginalSoundtrack);

            // set volumes
            SetMusicVolume(Settings.MusicVolume);
            SetSFXVolume(Settings.SFXVolume);

            // set the graphics quality
            QualitySettings.SetQualityLevel(Settings.CurrentQualityIndex, applyExpensiveChanges: true);

            // update any shaders
            TextureSetter.Instance.SetAllShaders(Settings.UseClassicShaders);

            // draw distance
            DreamSystem.ApplyEnvironment();

            Settings.UpdateCurrentProfile();

            OnSettingsApply.Raise();
            ProgrammaticOnSettingsApply?.Invoke();
        }

        public void SwitchToNextProfile()
        {
            Settings.SwitchToNextProfile();
            Apply();
        }

        public void RevertCurrentProfile()
        {
            Settings.ApplyCurrentProfile();
        }

        public void InvalidateCurrentProfile()
        {
            Settings.InvalidateCurrentProfile();
        }

        public void DeleteCurrentProfile()
        {
            Settings.DeleteCurrentProfile();
            Apply();
        }

        /// <summary>
        ///     Set the music volume.
        /// </summary>
        /// <param name="val">Music volume in percentage.</param>
        public void SetMusicVolume(float val) { MasterMixer.SetFloat("MusicVolume", volumeToDb(val)); }

        /// <summary>
        ///     Set the SFX volume.
        /// </summary>
        /// <param name="val">SFX volume in percentage.</param>
        public void SetSFXVolume(float val) { MasterMixer.SetFloat("SFXVolume", volumeToDb(val)); }

        // convert a volume percentage into decibels
        private static float volumeToDb(float volume)
        {
            if (volume <= 0) return -80;
            return 20 * Mathf.Log10(volume);
        }
    }
}
