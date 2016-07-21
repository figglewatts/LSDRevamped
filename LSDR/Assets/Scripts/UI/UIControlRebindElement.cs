using UnityEngine;
using System.Collections;
using InputManagement;
using UnityEngine.UI;

namespace UI
{
	public class UIControlRebindElement : MonoBehaviour
	{
		public Text ControlName;
		public Text InputName;

		// Use this for initialization
		void Start() { InputHandler.OnControlRebind += Rebound; }

		public void Rebind() { StartCoroutine(InputHandler.RebindKey(ControlName.text)); }

		private void Rebound(string buttonName, string newControl)
		{
			if (ControlName.text.Equals(buttonName))
			{
				InputName.text = newControl;
			}
		}
	}
}