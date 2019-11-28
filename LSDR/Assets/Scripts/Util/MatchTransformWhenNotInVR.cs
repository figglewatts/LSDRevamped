using UnityEngine;
using LSDR.Game;

namespace LSDR.Util
{
	/// <summary>
	/// Used to make things match the transform of another object when not in VR mode
	/// Mainly used to make the pause menu locked to camera unless we're in VR
	/// TODO: is MatchTransformWhenNotInVR obsolete?
	/// </summary>
	public class MatchTransformWhenNotInVR : MonoBehaviour
	{
		public SettingsSystem Settings;
		
		public Transform Target;
		public bool WhenInVR = false;
		public bool MatchPosition = true;

		void Update()
		{
			if (WhenInVR && !Settings.VR) return;

			if (MatchPosition) transform.position = Target.position;
			transform.rotation = Target.rotation;
		}
	}
}
