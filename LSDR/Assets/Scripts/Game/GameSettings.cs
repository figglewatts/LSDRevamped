using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Torii.Binding;
using UnityEngine;

namespace LSDR.Game
{
    /// <summary>
    ///     GameSettings is the class used for storing and serialisation/deserialisation of the game's settings.
    ///     It also contains numerous functions for applying game settings.
    /// </summary>
    [JsonObject]
    public class GameSettings : IPropertyWatcher
    {
        public int CurrentProfileIndex;

        public List<SettingsProfile> Profiles;
        public bool SettingsMatchProfile;

        protected ModLoaderSystem _modLoaderSystem;
        protected GameSaveSystem _gameSaveSystem;
        protected SettingsSystem _settingsSystem;

        /// <summary>
        ///     Create a new instance of the settings object with default values.
        /// </summary>
        public GameSettings(ModLoaderSystem modLoaderSystem, GameSaveSystem saveSystem, SettingsSystem settingsSystem)
        {
            _modLoaderSystem = modLoaderSystem;
            _gameSaveSystem = saveSystem;
            _settingsSystem = settingsSystem;

            HeadBobEnabled = true;
            CurrentControlSchemeIndex = 0;
            UseClassicShaders = true;
            UsePixelationShader = true;
            ResolutionWidth = 0;
            ResolutionHeight = 0;
            CurrentQualityIndex = QualitySettings.GetQualityLevel();
            Fullscreen = Screen.fullScreen;
            VSync = true;
            FOV = 60;
            FramerateLimit = 0;
            AffineIntensity = 0.5F;
            CurrentJournalIndex = 0;
            EnableFootstepSounds = true;
            MusicVolume = 1F;
            SFXVolume = 1F;
            UseOriginalSoundtrack = true;
            GUID = Guid.NewGuid();
            HeadBobIntensity = 1;
            SmoothHeadBob = false;
            LookSensitivity = 15;
            SpecialDaysEnabled = true;

            Profiles = new List<SettingsProfile>();
            SettingsMatchProfile = true;
        }

        public void ProvideModLoader(ModLoaderSystem modLoaderSystem)
        {
            _modLoaderSystem = modLoaderSystem;
        }

        public void ProvideSaveSystem(GameSaveSystem saveSystem)
        {
            _gameSaveSystem = saveSystem;
        }

        public void ProvideSettingsSystem(SettingsSystem settingsSystem)
        {
            _settingsSystem = settingsSystem;
        }

        [JsonIgnore]
        public SettingsProfile CurrentProfile => Profiles[CurrentProfileIndex];

        // used for BindBrokers
        public void NotifyPropertyChange(string propertyName)
        {
            OnPropertyChange?.Invoke(propertyName, this);
        }

        public void SwitchToNextProfile()
        {
            if (SettingsMatchProfile)
            {
                CurrentProfileIndex = (CurrentProfileIndex + 1) % Profiles.Count;
            }
            ApplyCurrentProfile();
        }

        public void ApplyCurrentProfile()
        {
            if (CurrentProfileIndex >= Profiles.Count || CurrentProfileIndex < 0) CurrentProfileIndex = 0;
            Profiles[CurrentProfileIndex].ApplyTo(this);
            SettingsMatchProfile = true;
        }

        public void UpdateCurrentProfile()
        {
            CurrentProfile.ApplyFrom(this);
            ApplyCurrentProfile();
        }

        public void InvalidateCurrentProfile()
        {
            SettingsMatchProfile = false;
        }

        public void CreateNewProfile()
        {
            Profiles.Add(new SettingsProfile(this) { Name = $"Profile {Profiles.Count}" });
            CurrentProfileIndex = Profiles.Count - 1;
            ApplyCurrentProfile();
        }

        public void DeleteCurrentProfile()
        {
            if (Profiles.Count == 1)
            {
                Debug.LogWarning("Cannot delete last profile");
                return;
            }

            Profiles.RemoveAt(CurrentProfileIndex);

            // if we deleted the last one, adjust it so we're now on the new last one
            if (CurrentProfileIndex == Profiles.Count)
            {
                CurrentProfileIndex--;
            }
            ApplyCurrentProfile();
        }

        public void DecrementJournal()
        {
            // check if we're on the first journal in this mod, if so we'll need to decrement the mod
            if (_currentJournalIndex == 0)
            {
                // check if we're on the first mod, if so we'll need to change to the last mod
                if (_currentModIndex == 0)
                {
                    _currentModIndex = _modLoaderSystem.ModsCount - 1;
                    NotifyPropertyChange(nameof(CurrentModIndex));
                }
                else
                {
                    // decrement the mod
                    _currentModIndex--;
                    NotifyPropertyChange(nameof(CurrentModIndex));
                }
                _gameSaveSystem.Load(); // load the save of this mod

                // set the journal index to the last journal in the mod
                _currentJournalIndex = _modLoaderSystem.Mods[_currentModIndex].Journals.Count - 1;
                NotifyPropertyChange(nameof(CurrentJournalIndex));
            }
            else
            {
                // otherwise, we can just decrement as normal as we're in the same mod
                _currentJournalIndex--;
                NotifyPropertyChange(nameof(CurrentJournalIndex));
            }
            _settingsSystem.Save(); // save the current setting of journal/mod
        }

        public void IncrementJournal()
        {
            // check if we're on the last journal in this mod, if so we'll need to increment the mod
            if (_currentJournalIndex == _modLoaderSystem.Mods[_currentModIndex].Journals.Count - 1)
            {
                // check if we're on the last mod, if so we'll need to change to the first mod
                if (_currentModIndex == _modLoaderSystem.ModsCount - 1)
                {
                    _currentModIndex = 0;
                    NotifyPropertyChange(nameof(CurrentModIndex));
                }
                else
                {
                    // increment the mod
                    _currentModIndex++;
                    NotifyPropertyChange(nameof(CurrentModIndex));
                }
                _gameSaveSystem.Load(); // load the save of this mod

                // set the journal index to the first in the mod
                _currentJournalIndex = 0;
                NotifyPropertyChange(nameof(CurrentJournalIndex));
            }
            else
            {
                // otherwise we can increment as normal as we've got another journal to go to
                _currentJournalIndex++;
                NotifyPropertyChange(nameof(CurrentJournalIndex));
            }
            _settingsSystem.Save(); // save the current setting of journal/mod
        }

#region Player Control Settings

        // private member of HeadBobEnabled
        private bool _headbobEnabled;

        /// <summary>
        ///     HeadBobEnabled is used to toggle player head bobbing.
        /// </summary>
        public bool HeadBobEnabled
        {
            get => _headbobEnabled;
            set
            {
                _headbobEnabled = value;
                NotifyPropertyChange(nameof(HeadBobEnabled));
            }
        }

        // private member of HeadBobEnabled
        private float _headbobIntensity;

        /// <summary>
        ///     HeadBobIntensity controls the intensity of head bobbing.
        /// </summary>
        public float HeadBobIntensity
        {
            get => _headbobIntensity;
            set
            {
                _headbobIntensity = value;
                NotifyPropertyChange(nameof(HeadBobIntensity));
            }
        }

        // private member of SmoothHeadBob
        private bool _smoothHeadBob;

        /// <summary>
        ///     Controls whether head bobbing is smoothed.
        /// </summary>
        public bool SmoothHeadBob
        {
            get => _smoothHeadBob;
            set
            {
                _smoothHeadBob = value;
                NotifyPropertyChange(nameof(SmoothHeadBob));
            }
        }

        // private member of LookSensitivity
        private float _lookSensitivity;

        // used for both mouse and stick look
        public float LookSensitivity
        {
            get => _lookSensitivity;
            set
            {
                _lookSensitivity = value;
                NotifyPropertyChange(nameof(LookSensitivity));
            }
        }

        // private member of CurrentControlSchemeIndex
        private int _currentControlSchemeIndex;

        /// <summary>
        ///     CurrentControlSchemeIndex is used to indicate which control scheme we're using.
        ///     Please see ControlSchemeManager for more detail.
        /// </summary>
        public int CurrentControlSchemeIndex
        {
            get => _currentControlSchemeIndex;
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
        ///     Whether or not classic (PS1) shaders are enabled.
        /// </summary>
        public bool UseClassicShaders
        {
            get => _useClassicShaders;
            set
            {
                _useClassicShaders = value;
                NotifyPropertyChange(nameof(UseClassicShaders));
            }
        }

        // private member of UsePixelationShader
        private bool _usePixelationShader;

        /// <summary>
        ///     Whether or not to pixelate the screen to the PS1 resolution.
        /// </summary>
        public bool UsePixelationShader
        {
            get => _usePixelationShader;
            set
            {
                _usePixelationShader = value;
                NotifyPropertyChange(nameof(UsePixelationShader));
            }
        }

        // private member of UseDithering
        private bool _useDithering;

        /// <summary>
        ///     Whether or not dithering is enabled on the pixelation shader.
        /// </summary>
        public bool UseDithering
        {
            get => _useDithering;
            set
            {
                _useDithering = value;
                NotifyPropertyChange(nameof(UseDithering));
            }
        }

        // 0x0 is native
        public int ResolutionWidth;
        public int ResolutionHeight;

        // old settings stored an index into Screen.resolutions, treat 0 as native
        [JsonProperty("CurrentResolutionIndex")]
        private int LegacyResolutionIndex
        {
            set
            {
                Resolution[] modes = Screen.resolutions;
                if (value <= 0 || value >= modes.Length) return;
                ResolutionWidth = modes[value].width;
                ResolutionHeight = modes[value].height;
            }
        }

        // index into the resolution dropdown
        [JsonIgnore]
        public int ResolutionOption
        {
            get => Mathf.Max(0, DisplayOptions.Resolutions.IndexOf(new Vector2Int(ResolutionWidth, ResolutionHeight)));
            set
            {
                Vector2Int size = DisplayOptions.Resolutions[Mathf.Clamp(value, 0, DisplayOptions.Resolutions.Count - 1)];
                ResolutionWidth = size.x;
                ResolutionHeight = size.y;
                NotifyPropertyChange(nameof(ResolutionOption));
            }
        }

        // private member of CurrentQualityIndex
        private int _currentQualityIndex;

        /// <summary>
        ///     The index into Unity's quality settings that we should be on.
        /// </summary>
        public int CurrentQualityIndex
        {
            get => _currentQualityIndex;
            set
            {
                _currentQualityIndex = value;
                NotifyPropertyChange(nameof(CurrentQualityIndex));
            }
        }

        // private member of Fullscreen
        private bool _fullscreen;

        /// <summary>
        ///     Whether or not we're currently in fullscreen mode.
        /// </summary>
        public bool Fullscreen
        {
            get => _fullscreen;
            set
            {
                _fullscreen = value;
                NotifyPropertyChange(nameof(Fullscreen));
            }
        }

        // private member of VSync
        private bool _vSync;

        // waits for the display between frames, so there's no tearing
        public bool VSync
        {
            get => _vSync;
            set
            {
                _vSync = value;
                NotifyPropertyChange(nameof(VSync));
            }
        }

        // private member of FOV
        private float _fov;

        /// <summary>
        ///     The camera FOV.
        /// </summary>
        public float FOV
        {
            get => _fov;
            set
            {
                _fov = value;
                NotifyPropertyChange(nameof(FOV));
            }
        }

        // 0 is uncapped
        public int FramerateLimit;

        // old settings only had an on/off limit
        [JsonProperty("LimitFramerate")]
        private bool LegacyLimitFramerate
        {
            set
            {
                if (value) FramerateLimit = DisplayOptions.ORIGINAL_FRAMERATE;
            }
        }

        // index into the framerate dropdown
        [JsonIgnore]
        public int FramerateOption
        {
            get => Mathf.Max(0, Array.IndexOf(DisplayOptions.FramerateLimits, FramerateLimit));
            set
            {
                FramerateLimit = DisplayOptions.FramerateLimits[Mathf.Clamp(value, 0, DisplayOptions.FramerateLimits.Length - 1)];
                NotifyPropertyChange(nameof(FramerateOption));
            }
        }

        private float _affineIntensity;

        /// <summary>
        ///     How intense to render the affine effect used in PS1 shaders.
        /// </summary>
        public float AffineIntensity
        {
            get => _affineIntensity;
            set
            {
                _affineIntensity = value;
                NotifyPropertyChange(nameof(AffineIntensity));
            }
        }

        private bool _longDrawDistance;

        public bool LongDrawDistance
        {
            get => _longDrawDistance;
            set
            {
                _longDrawDistance = value;
                NotifyPropertyChange(nameof(LongDrawDistance));
            }
        }

#endregion

#region Mod Settings

        private int _currentModIndex;

        public int CurrentModIndex
        {
            get => _currentModIndex;
            set
            {
                Debug.Log("Setting current mod to: " + value);
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
        ///     The index into the array of dream journals we're currently on. See DreamJournalManager.
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
        ///     Whether or not footstep sounds are enabled.
        /// </summary>
        public bool EnableFootstepSounds
        {
            get => _enableFootstepSounds;
            set
            {
                _enableFootstepSounds = value;
                NotifyPropertyChange(nameof(EnableFootstepSounds));
            }
        }

        // private member of MusicVolume
        private float _musicVolume;

        /// <summary>
        ///     The current volume of music.
        /// </summary>
        public float MusicVolume
        {
            get => _musicVolume;
            set
            {
                _musicVolume = value;
                NotifyPropertyChange(nameof(MusicVolume));
            }
        }

        // private member of SFXVolume
        private float _sfxVolume;

        /// <summary>
        ///     The current volume of SFX.
        /// </summary>
        public float SFXVolume
        {
            get => _sfxVolume;
            set
            {
                _sfxVolume = value;
                NotifyPropertyChange(nameof(SFXVolume));
            }
        }

        private bool _useOriginalSoundtrack;

        public bool UseOriginalSoundtrack
        {
            get => _useOriginalSoundtrack;
            set
            {
                _useOriginalSoundtrack = value;
                NotifyPropertyChange(nameof(UseOriginalSoundtrack));
            }
        }

#endregion

#region Gameplay Settings

        private bool _specialDaysEnabled;

        public bool SpecialDaysEnabled
        {
            get => _specialDaysEnabled;
            set
            {
                _specialDaysEnabled = value;
                NotifyPropertyChange(nameof(SpecialDaysEnabled));
            }
        }

#endregion

#region BindBroker fields

        public event Action<string, IPropertyWatcher> OnPropertyChange;

        [JsonIgnore]
        public Guid GUID { get; }

#endregion
    }
}
