using UnityEngine;
using InputManagement;
using IO;
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

		ControlSchemeManager.SwitchToScheme("Classic");
	}

	void Update()
	{
		// Do game update stuff here

		InputHandler.HandleInput();
	}
}
