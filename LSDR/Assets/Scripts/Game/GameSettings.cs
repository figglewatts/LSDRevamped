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

        /// <summary>
        ///     Create a new instance of the settings object with default values.
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
            UseOriginalSoundtrack = true;
            GUID = Guid.NewGuid();
            HeadBobIntensity = 1;
            SmoothHeadBob = false;
            SpecialDaysEnabled = true;

            Debug.Log($"HeadBobIntensity: {HeadBobIntensity}");

            Profiles = new List<SettingsProfile>();
            SettingsMatchProfile = true;
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

        // private member of CurrentResolutionIndex
        private int _currentResolutionIndex;

        /// <summary>
        ///     The index into Screen.resolutions that the current resolution is.
        /// </summary>
        public int CurrentResolutionIndex
        {
            get => _currentResolutionIndex;
            set
            {
                _currentResolutionIndex = value;
                NotifyPropertyChange(nameof(CurrentResolutionIndex));
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

        // private member of LimitFramerate
        private bool _limitFramerate;

        /// <summary>
        ///     Whether or not to limit the framerate to that of the PS1.
        /// </summary>
        public bool LimitFramerate
        {
            get => _limitFramerate;
            set
            {
                _limitFramerate = value;
                NotifyPropertyChange(nameof(LimitFramerate));
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
