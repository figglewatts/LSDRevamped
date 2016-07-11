using UnityEngine;
using System.Collections;
using InputManagement;

public class GameScript : MonoBehaviour
{
	void Start()
	{
		// do game startup stuff here

		// TODO: load current input scheme from game settings
		// TODO: if no settings are found load a default input scheme
		InputHandler.AddInput("Forward", KeyCode.W);
		InputHandler.AddInput("Backward", KeyCode.S);
		InputHandler.AddInput("Left", KeyCode.A);
		InputHandler.AddInput("Right", KeyCode.D);


		InputHandler.Initialize();
	}

	void Update()
	{
		// Do game update stuff here
		InputHandler.HandleInput();
	}
}
