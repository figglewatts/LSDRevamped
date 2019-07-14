using UnityEngine;
using System.Collections;
using LSDR.Entities.Dream;
using LSDR.Game;
using LSDR.Types;
using LSDR.UI;
using LSDR.Util;

namespace LSDR.Entities.Dream
{
	// TODO: DreamDirector refactoring, DreamPayloadReceiver
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