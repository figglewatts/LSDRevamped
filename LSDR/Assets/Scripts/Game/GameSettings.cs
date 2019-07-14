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
	/// <summary>
	/// GameSettings is the class used for storing and serialisation/deserialisation of the game's settings.
	/// It also contains numerous functions for applying game settings.
	/// </summary>
	[JsonObject]
    public class GameSettings : IPropertyWatcher
	{
		#region Player Control Settings

		// private member of HeadBobEnabled
	    private bool _headbobEnabled;

	    /// <summary>
	    /// HeadBobEnabled is used to toggle player head bobbing.
	    /// </summary>
	    public bool HeadBobEnabled
	    {
	        get { return _headbobEnabled; }
	        set
	        {
	            _headbobEnabled = value;
                NotifyPropertyChange(nameof(HeadBobEnabled));
	        }
	    }

	    // private member of CurrentControlSchemeIndex
	    private int _currentControlSchemeIndex;

	    /// <summary>
	    /// CurrentControlSchemeIndex is used to indicate which control scheme we're using.
	    /// Please see ControlSchemeManager for more detail.
	    /// </summary>
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

		// private member of UseClassicShaders
	    private bool _useClassicShaders;

	    /// <summary>
	    /// Whether or not classic (PS1) shaders are enabled.
	    /// </summary>
	    public bool UseClassicShaders
	    {
	        get { return _useClassicShaders; }
	        set
	        {
	            _useClassicShaders = value;
                NotifyPropertyChange(nameof(UseClassicShaders));
	        }
	    }

	    // private member of UsePixelationShader
	    private bool _usePixelationShader;

	    /// <summary>
	    /// Whether or not to pixelate the screen to the PS1 resolution.
	    /// </summary>
	    public bool UsePixelationShader
	    {
	        get { return _usePixelationShader; }
	        set
	        {
	            _usePixelationShader = value;
                NotifyPropertyChange(nameof(UsePixelationShader));
	        }
	    }

	    // private member of CurrentResolutionIndex
	    private int _currentResolutionIndex;
	    
	    /// <summary>
	    /// The index into Screen.resolutions that the current resolution is.
	    /// </summary>
	    public int CurrentResolutionIndex {
	        get { return _currentResolutionIndex; }
	        set
	        {
	            _currentResolutionIndex = value;
                NotifyPropertyChange(nameof(CurrentResolutionIndex));
	        }
	    }

	    // private member of CurrentQualityIndex
	    private int _currentQualityIndex;

	    /// <summary>
	    /// The index into Unity's quality settings that we should be on.
	    /// </summary>
	    public int CurrentQualityIndex
	    {
	        get { return _currentQualityIndex; }
	        set
	        {
	            _currentQualityIndex = value;
                NotifyPropertyChange(nameof(CurrentQualityIndex));
	        }
	    }

	    // private member of Fullscreen
	    private bool _fullscreen;

	    /// <summary>
	    /// Whether or not we're currently in fullscreen mode.
	    /// </summary>
	    public bool Fullscreen
	    {
	        get { return _fullscreen; }
	        set
	        {
	            _fullscreen = value;
                NotifyPropertyChange(nameof(Fullscreen));
	        }
	    }

	    // private member of FOV
	    private float _fov;

	    /// <summary>
	    /// The camera FOV.
	    /// </summary>
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

	    // private member of LimitFramerate
	    private bool _limitFramerate;

	    /// <summary>
	    /// Whether or not to limit the framerate to that of the PS1.
	    /// </summary>
	    public bool LimitFramerate
	    {
	        get { return _limitFramerate; }
	        set
	        {
	            _limitFramerate = value;
                NotifyPropertyChange(nameof(LimitFramerate));
	        }
	    }

	    /// <summary>
	    /// Hidden setting. How intense to render the affine effect used in PS1 shaders.
	    /// </summary>
        [JsonIgnore]
		public float AffineIntensity { get; set; } // the intensity of the affine texture mapping used in classic shaders

		#endregion

		#region Journal Settings

		// private member of CurrentJournalIndex
	    private int _currentJournalIndex;

	    /// <summary>
	    /// The index into the array of dream journals we're currently on. See DreamJournalManager.
	    /// </summary>
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

		// private member of EnableFootstepSounds
	    private bool _enableFootstepSounds;

	    /// <summary>
	    /// Whether or not footstep sounds are enabled.
	    /// </summary>
	    public bool EnableFootstepSounds
	    {
	        get { return _enableFootstepSounds; }
	        set
	        {
	            _enableFootstepSounds = value;
                NotifyPropertyChange(nameof(EnableFootstepSounds));
	        }
	    }

	    // private member of MusicVolume
	    private float _musicVolume;

	    /// <summary>
	    /// The current volume of music.
	    /// </summary>
	    public float MusicVolume
	    {
	        get { return _musicVolume; }
	        set
	        {
	            _musicVolume = value;
                NotifyPropertyChange(nameof(MusicVolume));
	        }
	    }

	    // private member of SFXVolume
	    private float _sfxVolume;

	    /// <summary>
	    /// The current volume of SFX.
	    /// </summary>
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

		/// <summary>
		/// Used to disable player motion, i.e. when linking.
		/// </summary>
        [JsonIgnore]
		public static bool CanControlPlayer = true;

		/// <summary>
		/// Used to disable mouse looking, i.e. when paused. Please use SetCursorViewState().
		/// </summary>
        [JsonIgnore]
		public static bool CanMouseLook = true;

		/// <summary>
		/// Whether or not the game is currently paused.
		/// </summary>
        [JsonIgnore]
		public static bool IsPaused = false;

		/// <summary>
		/// Whether or not we're in VR mode.
		/// </summary>
        [JsonIgnore]
		public static bool VR = !UnityEngine.XR.XRSettings.loadedDeviceName.Equals(string.Empty);

		/// <summary>
		/// Should the fog be additive or subtractive.
		/// TODO: move subtractive fog into dream environment?
		/// </summary>
		[JsonIgnore] public static bool SubtractiveFog = false;

		#endregion

		#region Gameplay Constants (not serialized)

		/// <summary>
		/// The framerate of the PS1. Used when framerate limiting is enabled.
		/// </summary>
        [JsonIgnore]
		public const int FRAMERATE_LIMIT = 25;

		/// <summary>
		/// Grey man will spawn approx 1 in every CHANCE_FOR_GREYMAN-1 times.
		/// TODO: put chance_for_greyman in dream data?
		/// </summary>
        [JsonIgnore]
		public const int CHANCE_FOR_GREYMAN = 100;

		#endregion

		// reference to master audio mixer used for volume controls
	    private static AudioMixer _masterMixer;

	    // reference to serializer used for loading/saving data
        private static readonly ToriiSerializer _serializer = new ToriiSerializer();

        // the path to the settings serialized file
	    private static string SettingsPath => PathUtil.Combine(Application.persistentDataPath, "settings.json");
	    
	    // the currently loaded settings
	    private static GameSettings _currentSettings;

	    #region BindBroker fields
        public event Action<string, IPropertyWatcher> OnPropertyChange;

        [JsonIgnore]
        public Guid GUID { get; }

        public static BindBroker SettingsBindBroker = new BindBroker();
        #endregion

		/// <summary>
		/// Get the currently loaded GameSettings, to access deserialized settings.
		/// </summary>
	    public static GameSettings CurrentSettings
	    {
	        get { return _currentSettings; }
	        set
	        {
	            _currentSettings = value;
                ApplySettings(value);
	        }
	    }

		/// <summary>
		/// Create a new instance of the settings object with default values.
		/// </summary>
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

		/// <summary>
		/// Static constructor to initialise the serializer.
		/// </summary>
        static GameSettings()
        {
            _serializer.RegisterJsonSerializationSettings(typeof(GameSettings), new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                SerializationBinder = new DefaultSerializationBinder()
            });
        }

		/// <summary>
		/// Set the cursor view state. True sets the cursor to visible and unlocks it, false does the inverse.
		/// </summary>
		/// <param name="state">Cursor state to set.</param>
		public static void SetCursorViewState(bool state)
		{
			Cursor.visible = state;
			Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;
		}

		/// <summary>
		/// Change the game's pause state. Pausing the game will enable the mouse pointer.
		/// </summary>
		/// <param name="pauseState">The pause state to set.</param>
		public static void PauseGame(bool pauseState)
		{
			IsPaused = pauseState;
			SetCursorViewState(pauseState);
			Time.timeScale = pauseState ? 0 : 1;
		}

		/// <summary>
		/// Apply the game settings. This function propagates the given settings to all game systems that need them.
		/// </summary>
		/// <param name="settings">The settings to apply.</param>
		public static void ApplySettings(GameSettings settings)
		{
            // TODO: try and catch exceptions for erroneous loaded values (i.e. array idx) and reset to default if error
		    
            // set the control scheme
		    ControlSchemeManager.UseScheme(settings.CurrentControlSchemeIndex);

		    // set the resolution
			if (settings.CurrentResolutionIndex > Screen.resolutions.Length)
			{
			    // if the resolution is invalid, set it to the lowest resolution
				Screen.SetResolution(Screen.resolutions[0].width, Screen.resolutions[0].height, settings.Fullscreen);
			}
			else
			{
			    Screen.SetResolution(Screen.resolutions[settings.CurrentResolutionIndex].width,
			        Screen.resolutions[settings.CurrentResolutionIndex].height, settings.Fullscreen);
			}
			
			// set framerate to limit or not
			Application.targetFrameRate = settings.LimitFramerate ? FRAMERATE_LIMIT : -1;
			
			// set retro shader affine intensity
			Shader.SetGlobalFloat("AffineIntensity", settings.AffineIntensity);
			
			// set the current dream journal
			DreamJournalManager.SetJournal(settings.CurrentJournalIndex);
			
			// set volumes
			SetMusicVolume(settings.MusicVolume);
            SetSFXVolume(settings.SFXVolume);
            
            // set the graphics quality
            QualitySettings.SetQualityLevel(settings.CurrentQualityIndex, true);

            Debug.Log("Applying game settings...");
            Debug.Log("Affine intensity: " + settings.AffineIntensity);
		}

		/// <summary>
		/// Load the saved settings from the file.
		/// </summary>
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

		/// <summary>
		/// Save the given settings to the settings file.
		/// </summary>
		/// <param name="settings">The settings to save.</param>
		public static void SaveSettings(GameSettings settings)
		{
            Debug.Log("Saving game settings...");

		    _serializer.Serialize(settings, SettingsPath);
		}

		/// <summary>
		/// Set the music volume.
		/// </summary>
		/// <param name="val">Music volume in percentage.</param>
        public static void SetMusicVolume(float val)
        {
            _masterMixer.SetFloat("MusicVolume", volumeToDb(val));
        }

		/// <summary>
		/// Set the SFX volume.
		/// </summary>
		/// <param name="val">SFX volume in percentage.</param>
        public static void SetSFXVolume(float val)
        {
            _masterMixer.SetFloat("SFXVolume", volumeToDb(val));
        }

		// find the current resolution index
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

        // convert a volume percentage into decibels
        private static float volumeToDb(float volume)
        {
            if (volume <= 0) return -80;
            return 20 * Mathf.Log10(volume);
        }

        /// <summary>
        /// Initialize settings. Should be called on game startup.
        /// </summary>
	    public static void Initialize()
	    {
	        _masterMixer = Resources.Load<AudioMixer>("Mixers/MasterMixer");
	    }

        // used for BindBrokers
	    public void NotifyPropertyChange(string propertyName) { OnPropertyChange?.Invoke(propertyName, this); }
	}
}
