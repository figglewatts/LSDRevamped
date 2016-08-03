using UnityEngine;
using System.Collections;
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

		void Update()
		{
			if (VRSettings.loadedDeviceName.Equals(string.Empty)) return;

			transform.position = Target.position;
			transform.rotation = Target.rotation;
		}
	}
}
