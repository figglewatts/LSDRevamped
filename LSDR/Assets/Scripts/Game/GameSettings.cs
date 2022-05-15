using System;
using Newtonsoft.Json;
using Torii.Binding;
using UnityEngine;

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
        public int CurrentResolutionIndex
        {
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
        public float
            AffineIntensity { get; set; } // the intensity of the affine texture mapping used in classic shaders

#endregion

#region Mod Settings

        private int _currentModIndex;

        public int CurrentModIndex
        {
            get => _currentModIndex;
            set
            {
                _currentModIndex = value;
                _currentJournalIndex = 0;
                NotifyPropertyChange(nameof(CurrentJournalIndex));
                NotifyPropertyChange(nameof(CurrentModIndex));
            }
        }

#endregion

#region Journal Settings

        // private member of CurrentJournalIndex
        private int _currentJournalIndex;

        /// <summary>
        /// The index into the array of dream journals we're currently on. See DreamJournalManager.
        /// </summary>
        public int CurrentJournalIndex
        {
            get => _currentJournalIndex;
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

#region BindBroker fields

        public event Action<string, IPropertyWatcher> OnPropertyChange;

        [JsonIgnore]
        public Guid GUID { get; }

#endregion


        /// <summary>
        /// Create a new instance of the settings object with default values.
        /// </summary>
        public GameSettings()
        {
            HeadBobEnabled = true;
            CurrentControlSchemeIndex = 0;
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

        // used for BindBrokers
        public void NotifyPropertyChange(string propertyName) { OnPropertyChange?.Invoke(propertyName, this); }
    }
}
