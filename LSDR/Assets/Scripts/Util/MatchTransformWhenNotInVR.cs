using UnityEngine;
using System.Collections;
using Game;
using UnityEngine.VR;

namespace Util
{
	/// <summary>
	/// Used to make things match the transform of another object when not in VR mode
	/// Mainly used to make the pause menu locked to camera unless we're in VR
	/// </summary>
	public class MatchTransformWhenNotInVR : MonoBehaviour
	{
		public Transform Target;
		public bool WhenInVR = false;
		public bool MatchPosition = true;

		void Update()
		{
			if (WhenInVR && !GameSettings.VR) return;

			if (MatchPosition) transform.position = Target.position;
			transform.rotation = Target.rotation;
		}
	}
}
