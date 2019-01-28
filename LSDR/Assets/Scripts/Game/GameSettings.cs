using System;
using System.IO;
using InputManagement;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SimpleJSON;
using Torii.Binding;
using Torii.Serialization;
using Torii.Util;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.VR;
using Util;

namespace Game
{
	[JsonObject]
    public class GameSettings : IPropertyWatcher
	{
		#region Player Control Settings

		// modifiable settings
	    private bool _headbobEnabled;

	    public bool HeadBobEnabled
	    {
	        get { return _headbobEnabled; }
	        set
	        {
	            _headbobEnabled = value;
                NotifyPropertyChange(nameof(HeadBobEnabled));
	        }
	    }

	    private int _currentControlSchemeIndex;

	    public int CurrentControlSchemeIndex
	    {
	        get { return _currentControlSchemeIndex; }
	        set
	        {
	            _currentControlSchemeIndex = value;
                NotifyPropertyChange(nameof(CurrentControlSchemeIndex));
	        }
	    }

		#endregion

		#region Graphical Settings

		// modifiable settings
	    private bool _useClassicShaders;

	    public bool UseClassicShaders
	    {
	        get { return _useClassicShaders; }
	        set
	        {
	            _useClassicShaders = value;
                NotifyPropertyChange(nameof(UseClassicShaders));
	        }
	    }

	    private bool _usePixelationShader;

	    public bool UsePixelationShader
	    {
	        get { return _usePixelationShader; }
	        set
	        {
	            _usePixelationShader = value;
                NotifyPropertyChange(nameof(UsePixelationShader));
	        }
	    }

	    private int _currentResolutionIndex;
	    public int CurrentResolutionIndex {
	        get { return _currentResolutionIndex; }
	        set
	        {
	            _currentResolutionIndex = value;
                NotifyPropertyChange(nameof(CurrentResolutionIndex));
	        }
	    }

	    private int _currentQualityIndex;

	    public int CurrentQualityIndex
	    {
	        get { return _currentQualityIndex; }
	        set
	        {
	            _currentQualityIndex = value;
                NotifyPropertyChange(nameof(CurrentQualityIndex));
	        }
	    }

	    private bool _fullscreen;

	    public bool Fullscreen
	    {
	        get { return _fullscreen; }
	        set
	        {
	            _fullscreen = value;
                NotifyPropertyChange(nameof(Fullscreen));
	        }
	    }

	    private float _fov;

	    public float FOV
	    {
	        get { return _fov; }
	        set
	        {
	            _fov = value;
                Debug.Log("Setting FOV to " + _fov);
                NotifyPropertyChange(nameof(FOV));
	        }
	    }

	    private bool _limitFramerate;

	    public bool LimitFramerate
	    {
	        get { return _limitFramerate; }
	        set
	        {
	            _limitFramerate = value;
                NotifyPropertyChange(nameof(LimitFramerate));
	        }
	    }

	    // hidden settings
        [JsonIgnore]
		public float AffineIntensity { get; set; } // the intensity of the affine texture mapping used in classic shaders

		#endregion

		#region Journal Settings

	    private int _currentJournalIndex;

	    public int CurrentJournalIndex
	    {
	        get { return _currentJournalIndex; }
	        set
	        {
	            _currentJournalIndex = value;
                NotifyPropertyChange(nameof(CurrentJournalIndex));
	        }
	    }

#endregion

		#region Audio Settings

	    private bool _enableFootstepSounds;

	    public bool EnableFootstepSounds
	    {
	        get { return _enableFootstepSounds; }
	        set
	        {
	            _enableFootstepSounds = value;
                NotifyPropertyChange(nameof(EnableFootstepSounds));
	        }
	    }

	    private float _musicVolume;

	    public float MusicVolume
	    {
	        get { return _musicVolume; }
	        set
	        {
	            _musicVolume = value;
                NotifyPropertyChange(nameof(MusicVolume));
	        }
	    }

	    private float _sfxVolume;

	    public float SFXVolume
	    {
	        get { return _sfxVolume; }
	        set
	        {
	            _sfxVolume = value;
                NotifyPropertyChange(nameof(SFXVolume));
	        }
	    }

#endregion

		#region Global Gameplay Settings (not serialized)

        [JsonIgnore]
		public static bool CanControlPlayer = true; // used to disable character motion, i.e. when linking

        [JsonIgnore]
		public static bool CanMouseLook = true; // used to disable mouse looking, i.e. when paused

        [JsonIgnore]
		public static bool IsPaused = false;

        [JsonIgnore]
		public static bool VR = !UnityEngine.XR.XRSettings.loadedDeviceName.Equals(string.Empty);

		// should the fog mode be additive or subtractive
		[JsonIgnore] public static bool SubtractiveFog = false;

		#endregion

		#region Gameplay Constants (not serialized)

        [JsonIgnore]
		public const int FRAMERATE_LIMIT = 25;

		// grey man will spawn approx 1 in every CHANCE_FOR_GREYMAN-1 times
        [JsonIgnore]
		public const int CHANCE_FOR_GREYMAN = 100;

		#endregion

	    private static AudioMixer _masterMixer;

        private static readonly ToriiSerializer _serializer = new ToriiSerializer();

	    private static string SettingsPath => PathUtil.Combine(Application.persistentDataPath, "settings.json");

	    private static GameSettings _currentSettings;

        public event Action<string, IPropertyWatcher> OnPropertyChange;

        [JsonIgnore]
        public Guid GUID { get; }

        public static BindBroker SettingsBindBroker = new BindBroker();

	    public static GameSettings CurrentSettings
	    {
	        get { return _currentSettings; }
	        set
	        {
	            _currentSettings = value;
                ApplySettings(value);
	        }
	    }

	    public GameSettings()
	    {
	        HeadBobEnabled = true;
	        CurrentControlSchemeIndex = 0;
	        CanControlPlayer = true;
	        UseClassicShaders = true;
	        UsePixelationShader = true;
	        CurrentResolutionIndex = FindResolutionIndex();
	        CurrentQualityIndex = QualitySettings.GetQualityLevel();
	        Fullscreen = Screen.fullScreen;
	        FOV = 60;
	        LimitFramerate = false;
	        AffineIntensity = 0.5F;
	        CurrentJournalIndex = 0;
	        EnableFootstepSounds = true;
	        MusicVolume = 1F;
	        SFXVolume = 1F;
	        GUID = Guid.NewGuid();
	    }

        static GameSettings()
        {
            _serializer.RegisterJsonSerializationSettings(typeof(GameSettings), new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                SerializationBinder = new DefaultSerializationBinder()
            });
        }

		public static void SetCursorViewState(bool state)
		{
			Cursor.visible = state;
			Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;
		}

		public static void PauseGame(bool pauseState)
		{
			IsPaused = pauseState;
			SetCursorViewState(pauseState);
			Time.timeScale = pauseState ? 0 : 1;
		}

		public static void ApplySettings(GameSettings settings)
		{
            // TODO: try and catch exceptions for erroneous loaded values (i.e. array idx) and reset to default if error
		    
		    ControlSchemeManager.UseScheme(settings.CurrentControlSchemeIndex);

			if (settings.CurrentResolutionIndex > Screen.resolutions.Length)
			{
			    Screen.SetResolution(Screen.resolutions[0].width, Screen.resolutions[0].height, settings.Fullscreen);
			}
			else
			{
			    Screen.SetResolution(Screen.resolutions[settings.CurrentResolutionIndex].width,
			        Screen.resolutions[settings.CurrentResolutionIndex].height, settings.Fullscreen);
			}
			Application.targetFrameRate = settings.LimitFramerate ? FRAMERATE_LIMIT : -1;
			Shader.SetGlobalFloat("AffineIntensity", settings.AffineIntensity);
			DreamJournalManager.SetJournal(settings.CurrentJournalIndex);
			SetMusicVolume(settings.MusicVolume);
            SetSFXVolume(settings.SFXVolume);
            QualitySettings.SetQualityLevel(settings.CurrentQualityIndex, true);

            Debug.Log("Applying game settings...");
            Debug.Log("Affine intensity: " + settings.AffineIntensity);
		}

		public static void LoadSettings()
		{
		    Debug.Log("Loading game settings...");
		    
		    // check to see if the settings file exists
		    if (File.Exists(SettingsPath))
		    {
		        CurrentSettings = _serializer.Deserialize<GameSettings>(SettingsPath);
		    }
		    else
		    {
		        // create the default settings
                Debug.Log("Settings.json not found, creating default settings");
                CurrentSettings = new GameSettings();
                SaveSettings(CurrentSettings);
		    }
		}

		public static void SaveSettings(GameSettings settings)
		{
            Debug.Log("Saving game settings...");

		    _serializer.Serialize(settings, SettingsPath);
		}

        public static void SetMusicVolume(float val)
        {
            _masterMixer.SetFloat("MusicVolume", volumeToDb(val));
        }

        public static void SetSFXVolume(float val)
        {
            _masterMixer.SetFloat("SFXVolume", volumeToDb(val));
        }

        private static int FindResolutionIndex()
		{
			Resolution[] resolutions = Screen.resolutions;
			for (int i = 0; i < resolutions.Length; i++)
			{
				if (resolutions[i].Equals(Screen.currentResolution)) return i;
			}
			Debug.LogWarning("Could not find screen resolution!");
			return 0;
		}

        private static float volumeToDb(float volume)
        {
            if (volume <= 0) return -80;
            return 20 * Mathf.Log10(volume);
        }

	    public static void Initialize()
	    {
	        _masterMixer = Resources.Load<AudioMixer>("Mixers/MasterMixer");
	    }

	    public void NotifyPropertyChange(string propertyName) { OnPropertyChange?.Invoke(propertyName, this); }
	}
}
