using System;
using System.Collections;
using InControl;
using LSDR.InputManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LSDR.UI
{
	/// <summary>
	/// When attached to a UI object, makes it so that this object is initially selected.
	/// </summary>
	public class UISelectMe : MonoBehaviour
	{
		public bool SelectEvenWithMouse;
		public ControlSchemeLoaderSystem ControlScheme;
		
		void Start()
		{
			init();
			StartCoroutine(waitFrameThenSelect(InputManager.ActiveDevice));
		}

		private void Update()
		{
			if (ControlScheme.Current.Actions.Move.Value.sqrMagnitude > 0 &&
			    EventSystem.current.currentSelectedGameObject == null)
			{
				StartCoroutine(waitFrameThenSelect(InputManager.ActiveDevice));
			}
		}

		void OnEnable() { StartCoroutine(waitFrameThenSelect(InputManager.ActiveDevice)); }

		private void OnDestroy()
		{
			InputManager.OnActiveDeviceChanged -= onControllerConnect;
			InputManager.OnDeviceAttached -= onControllerConnect;
		}

		private void init()
		{
			InputManager.OnActiveDeviceChanged += onControllerConnect;
			InputManager.OnDeviceAttached += onControllerConnect;
		}

		private void onControllerConnect(InputDevice device) { StartCoroutine(waitFrameThenSelect(device)); }

		private IEnumerator waitFrameThenSelect(InputDevice device)
		{
			yield return null;
			if (!InputManager.ActiveDevice.Name.Equals("None") || SelectEvenWithMouse)
			{
				GetComponent<Selectable>().Select();
			}
		}
	}
}