using UnityEngine;
using System.Collections;
using Entities.Dream;
using Types;
using UI;
using Util;

public class PayloadReceiver : MonoBehaviour
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

		Fader.FadeOut(1F);
	}

	// Update is called once per frame
	void Update()
	{

	}
}
