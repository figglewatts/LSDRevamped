using UnityEngine;

namespace Game
{
	public static class GameSettings
	{
		// TODO: load from file

		#region Player Control Settings

		// modifiable settings
		public static bool HeadBobEnabled = true;
		public static bool FPSMovementEnabled = true;
		public static float MouseSensitivityX = 1F;
		public static float MouseSensitivityY = 1F;

		public static int CurrentControlScheme = 0;

		// hidden settings
		public static bool CanControlPlayer = true; // used to disable character motion, i.e. when linking

		#endregion

		#region Graphical Settings

		// modifiable settings
		public static bool UseClassicShaders = true;

		#endregion

		#region Journal Settings

		public static string CurrentJournalDir = "/";

		#endregion

		public static void SetCursorViewState(bool state)
		{
			Cursor.visible = state;
			Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;
		}
	}
}
