using UnityEngine;
using System.Collections;

namespace Util
{
	// TODO: try and find out where LockToPosition is used
	public class LockToPosition : MonoBehaviour
	{
		public Transform Target;

		void Update() { transform.position = Target.position; }
	}
}