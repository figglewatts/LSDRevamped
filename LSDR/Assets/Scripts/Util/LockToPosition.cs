using UnityEngine;
using System.Collections;

namespace Util
{
	public class LockToPosition : MonoBehaviour
	{
		public Transform Target;

		void Update() { transform.position = Target.position; }
	}
}