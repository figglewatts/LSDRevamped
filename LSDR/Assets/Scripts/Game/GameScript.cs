using Game;
using UnityEngine;
using InputManagement;
using IO;
using Types;
using UI;
using Util;

public class GameScript : MonoBehaviour
{
	void Start()
	{
		// do game startup stuff here

		InputHandler.AddInput("Forward", KeyCode.W);
		InputHandler.AddInput("Backward", KeyCode.S);
		InputHandler.AddInput("Left", KeyCode.A);
		InputHandler.AddInput("Right", KeyCode.D);
		InputHandler.AddInput("LookUp", KeyCode.Q);
		InputHandler.AddInput("LookDown", KeyCode.E);
		InputHandler.AddInput("Sprint", KeyCode.Space);

		InputHandler.Initialize();

		ControlSchemeManager.Initialize();

		DreamJournalManager.Initialize();

		MapReader.MapScaleFactor = 1F;

		GameSettings.LoadSettings();

		Shader.SetGlobalFloat("_FogStep", 0.08F);
	}

	void Update()
	{
		// Do game update stuff here

		InputHandler.HandleInput();
	}
}
