using System;
using System.IO;
using LSDR.InputManagement;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Torii.Binding;
using Torii.Serialization;
using Torii.Util;
using UnityEngine;
using UnityEngine.Audio;

namespace LSDR.Game
{
    [CreateAssetMenu(menuName="System/SettingsSystem")]
    public class SettingsSystem : ScriptableObject
    {
        // reference to master audio mixer used for volume controls
        public AudioMixer MasterMixer;
        
        public GameSettings Settings { get; private set; }
        
        /// <summary>
        /// Used to disable player motion, i.e. when linking.
        /// </summary>
        public bool CanControlPlayer = true;

        /// <summary>
        /// Used to disable mouse looking, i.e. when paused. Please use SetCursorViewState().
        /// </summary>
        public bool CanMouseLook = true;

        public BindBroker SettingsBindBroker = new BindBroker();

        /// <summary>
        /// Whether or not we're in VR mode.
        /// </summary>
        public bool VR;
        
        /// <summary>
        /// The framerate of the PS1. Used when framerate limiting is enabled.
        /// </summary>
        public const int FRAMERATE_LIMIT = 25;

        public Shader ClassicDiffuse;
        public Shader ClassicAlpha;
        public Shader RevampedDiffuse;
        public Shader RevampedAlpha;

        public Material[] DiffuseMaterialsInUse;
        public Material[] AlphaMaterialsInUse;

        public JournalLoaderSystem JournalLoader;
        public ControlSchemeLoaderSystem ControlSchemeLoader;

        // reference to serializer used for loading/saving data
        private readonly ToriiSerializer _serializer = new ToriiSerializer();

        // the path to the settings serialized file
        private static string SettingsPath => PathUtil.Combine(Application.persistentDataPath, "settings.json");

        public void OnEnable()
        {
            VR = !UnityEngine.XR.XRSettings.loadedDeviceName.Equals(string.Empty);
            
            _serializer.RegisterJsonSerializationSettings(typeof(GameSettings), new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                SerializationBinder = new DefaultSerializationBinder()
            });
        }

        public void Load()
        {
            Debug.Log("Loading game settings...");

            // if we're loading over an existing settings object we want to deregister the old one
            if (Settings != null)
            {
                SettingsBindBroker.DeregisterData(Settings);
            }
		    
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
            
            // register the new settings object
            SettingsBindBroker.RegisterData(Settings);
        }

        public void Save()
        {
            Debug.Log("Saving game settings...");

            _serializer.Serialize(Settings, SettingsPath);
        }
        
        /// <summary>
        /// Apply the game settings. This function propagates the given settings to all game systems that need them.
        /// </summary>
        public void Apply()
        {
            Debug.Log("Applying game settings");
            
            // TODO: try and catch exceptions for erroneous loaded values (i.e. array idx) and reset to default if error
		    
            // set the control scheme
            ControlSchemeLoader.SelectScheme(Settings.CurrentControlSchemeIndex);
            ControlSchemeLoader.SaveSchemes();

            // set the resolution
            if (Settings.CurrentResolutionIndex > Screen.resolutions.Length)
            {
                // if the resolution is invalid, set it to the lowest resolution
                Screen.SetResolution(Screen.resolutions[0].width, Screen.resolutions[0].height, Settings.Fullscreen);
            }
            else
            {
                Screen.SetResolution(Screen.resolutions[Settings.CurrentResolutionIndex].width,
                    Screen.resolutions[Settings.CurrentResolutionIndex].height, Settings.Fullscreen);
            }
			
            // set framerate to limit or not
            Application.targetFrameRate = Settings.LimitFramerate ? FRAMERATE_LIMIT : -1;
			
            // set retro shader affine intensity
            Shader.SetGlobalFloat("AffineIntensity", Settings.AffineIntensity);
			
            // set the current dream journal
            JournalLoader.SelectJournal(Settings.CurrentJournalIndex);
			
            // set volumes
            SetMusicVolume(Settings.MusicVolume);
            SetSFXVolume(Settings.SFXVolume);
            
            // set the graphics quality
            QualitySettings.SetQualityLevel(Settings.CurrentQualityIndex, true);

            // update any shaders
            foreach (var mat in DiffuseMaterialsInUse)
            {
                mat.shader = Settings.UseClassicShaders ? ClassicDiffuse : RevampedDiffuse;
            }
            foreach (var mat in AlphaMaterialsInUse)
            {
                mat.shader = Settings.UseClassicShaders ? ClassicAlpha : RevampedAlpha;
            }
        }

        /// <summary>
        /// Set the music volume.
        /// </summary>
        /// <param name="val">Music volume in percentage.</param>
        public void SetMusicVolume(float val)
        {
            MasterMixer.SetFloat("MusicVolume", volumeToDb(val));
        }

        /// <summary>
        /// Set the SFX volume.
        /// </summary>
        /// <param name="val">SFX volume in percentage.</param>
        public void SetSFXVolume(float val)
        {
            MasterMixer.SetFloat("SFXVolume", volumeToDb(val));
        }
        
        // convert a volume percentage into decibels
        private static float volumeToDb(float volume)
        {
            if (volume <= 0) return -80;
            return 20 * Mathf.Log10(volume);
        }
    }
}
