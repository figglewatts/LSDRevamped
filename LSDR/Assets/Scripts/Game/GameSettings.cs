using InputManagement;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Audio;
using Util;

namespace Game
{
	public static class GameSettings
	{
		// TODO: load from file

		#region Player Control Settings

		// modifiable settings
		public static bool HeadBobEnabled;
		public static int CurrentControlSchemeIndex;

		// hidden settings
		

		#endregion

		#region Graphical Settings

		// modifiable settings
		public static bool UseClassicShaders;
		public static bool UsePixelationShader;
		public static int CurrentResolutionIndex;
		public static int CurrentQualityIndex;
		public static bool Fullscreen;
		public static float FOV;
		public static bool LimitFramerate;
		
		// hidden settings
		public static float AffineIntensity; // the intensity of the affine texture mapping used in classic shaders

		#endregion

		#region Journal Settings

		public static int CurrentJournalIndex = 0;

		#endregion

		#region Audio Settings

		public static bool EnableFootstepSounds;
		public static float MusicVolume;
		public static float SFXVolume;

		#endregion

		#region Global Gameplay Settings (not serialized)

		public static bool CanControlPlayer = true; // used to disable character motion, i.e. when linking
		public static bool CanMouseLook = true; // used to disable mouse looking, i.e. when paused
		public static bool IsPaused = false;

		#endregion

		#region Gameplay Constants

		public const int FRAMERATE_LIMIT = 25;

		#endregion

		private static AudioMixer _masterMixer = Resources.Load<AudioMixer>("Mixers/MasterMixer");

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

		public static void SetDefaultSettings()
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
		}

		public static void ApplySettings()
		{
			ControlSchemeManager.SwitchToScheme(CurrentControlSchemeIndex);
			Screen.SetResolution(Screen.resolutions[CurrentResolutionIndex].width, Screen.resolutions[CurrentResolutionIndex].height, Fullscreen);
			Application.targetFrameRate = LimitFramerate ? FRAMERATE_LIMIT : -1;
			Shader.SetGlobalFloat("AffineIntensity", AffineIntensity);
			DreamJournalManager.SwitchJournal(CurrentJournalIndex);
			_masterMixer.SetFloat("MusicVolume", -80 + (MusicVolume*80));
			_masterMixer.SetFloat("SFXVolume", -80 + (SFXVolume * 80));
		}

		public static void LoadSettings()
		{
			JSONClass settingsJson;
			try
			{
				settingsJson =
					ResourceManager.Load<JSONClass>(IOUtil.PathCombine(Application.persistentDataPath, "Settings", "settings.json"),
						ResourceLifespan.GLOBAL, true);
			}
			catch (ResourceManager.ResourceLoadException e)
			{
				// must be first startup or settings has been deleted somehow
				Debug.Log("First startup, initializing settings!");
				SetDefaultSettings();
				SaveSettings();
				ApplySettings();
				return;
			}

			HeadBobEnabled = settingsJson["controls"]["headbob"].AsBool;
			CurrentControlSchemeIndex = settingsJson["controls"]["currentControlSchemeIndex"].AsInt;

			UseClassicShaders = settingsJson["graphics"]["classicShaders"].AsBool;
			UsePixelationShader = settingsJson["graphics"]["pixelationShader"].AsBool;
			CurrentResolutionIndex = settingsJson["graphics"]["currentResolutionIndex"].AsInt;
			CurrentQualityIndex = settingsJson["graphics"]["currentQualityIndex"].AsInt;
			Fullscreen = settingsJson["graphics"]["fullscreen"].AsBool;
			FOV = settingsJson["graphics"]["fov"].AsFloat;
			LimitFramerate = settingsJson["graphics"]["limitFramerate"].AsBool;
			AffineIntensity = settingsJson["graphics"]["affineIntensity"].AsFloat;

			CurrentJournalIndex = settingsJson["content"]["currentJournalIndex"].AsInt;

			EnableFootstepSounds = settingsJson["audio"]["footstepSounds"].AsBool;
			MusicVolume = settingsJson["audio"]["musicVolume"].AsFloat;
			SFXVolume = settingsJson["audio"]["sfxVolume"].AsFloat;

			ApplySettings();
		}

		public static void SaveSettings()
		{
			JSONClass settingsJson = new JSONClass();
			
			settingsJson["controls"]["headbob"].AsBool = HeadBobEnabled;
			settingsJson["controls"]["currentControlSchemeIndex"].AsInt = CurrentControlSchemeIndex;

			settingsJson["graphics"]["classicShaders"].AsBool = UseClassicShaders;
			settingsJson["graphics"]["pixelationShader"].AsBool = UsePixelationShader;
			settingsJson["graphics"]["currentResolutionIndex"].AsInt = CurrentResolutionIndex;
			settingsJson["graphics"]["currentQualityIndex"].AsInt = CurrentQualityIndex;
			settingsJson["graphics"]["fullscreen"].AsBool = Fullscreen;
			settingsJson["graphics"]["fov"].AsFloat = FOV;
			settingsJson["graphics"]["limitFramerate"].AsBool = LimitFramerate;
			settingsJson["graphics"]["affineIntensity"].AsFloat = AffineIntensity;

			settingsJson["content"]["currentJournalIndex"].AsInt = CurrentJournalIndex;

			settingsJson["audio"]["footstepSounds"].AsBool = EnableFootstepSounds;
			settingsJson["audio"]["musicVolume"].AsFloat = MusicVolume;
			settingsJson["audio"]["sfxVolume"].AsFloat = SFXVolume;

			IOUtil.WriteJSONToDisk(settingsJson, IOUtil.PathCombine(Application.persistentDataPath, "Settings", "settings.json"));
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
	}
}
