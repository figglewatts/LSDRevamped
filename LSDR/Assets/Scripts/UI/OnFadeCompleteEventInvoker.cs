using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UI
{
	/// <summary>
	/// Used by animation events to invoke post-fade delegates
	/// </summary>
	public class OnFadeCompleteEventInvoker : MonoBehaviour
	{
		public void InvokeOnFadeComplete() { Fader.InvokeOnFadeComplete(); }
	}
}
