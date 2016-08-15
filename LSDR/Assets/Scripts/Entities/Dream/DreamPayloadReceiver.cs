using UnityEngine;
using System.Collections;
using Entities.Dream;
using Game;
using Types;
using UI;
using Util;

namespace Entities.Dream
{
	public class DreamPayloadReceiver : MonoBehaviour
	{
		private DreamPayload _receivedPayload;

		// Use this for initialization
		void Start()
		{
			_receivedPayload = GameObject.FindGameObjectWithTag("DreamPayload").GetComponent<DreamPayload>();

			DreamDirector.Player = GameObject.FindGameObjectWithTag("Player");

			if (_receivedPayload == null)
			{
				Debug.LogError("Could not find dream payload! Did you come from the title screen?");
				return;
			}

			DreamDirector.SwitchDreamLevel(_receivedPayload.InitialLevelToLoad);

			GameSettings.SetCursorViewState(false);

			GameSettings.CanControlPlayer = true;

			Fader.FadeOut(1F);
		}
	}
}