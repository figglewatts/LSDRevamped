using UnityEngine;
using System.Collections;
using InputManagement;
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

	public void SetTextureSet(int set)
	{
		Material testA = Resources.Load<Material>("Materials/psx_vertexlit_test");
		Material testB = Resources.Load<Material>("Materials/psx_vertexlit_transparent_test");
		testA.SetInt("_TextureSet", set);
		testB.SetInt("_TextureSet", set);
	}
}
