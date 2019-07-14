using UnityEngine;

namespace LSDR.UI
{
	/// <summary>
	/// Used by animation events to invoke post-fade delegates.
	/// </summary>
	public class OnFadeCompleteEventInvoker : MonoBehaviour
	{
		public void InvokeOnFadeComplete() { Fader.InvokeOnFadeComplete(); }
	}
}
