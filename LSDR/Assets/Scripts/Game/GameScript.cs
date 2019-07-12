using System.Reflection;
using Game;
using UnityEngine;
using InputManagement;
using IO;
using ResourceHandlers;
using Visual;
using ResourceManager = Torii.Resource.ResourceManager;

[assembly: AssemblyVersion("0.1.*")]
/// <summary>
/// GameScript is essentially the entrypoint to the game. All initialisation stuff is done in the Start() method here.
/// It will be executed before MOST scripts.
/// </summary>
public class GameScript : MonoBehaviour
{
	void Start()
	{
		// do game startup stuff here

        GameSettings.Initialize();
        
        ResourceManager.Initialize();
        ResourceManager.RegisterHandler(new LBDHandler());
        ResourceManager.RegisterHandler(new TIXHandler());
        ResourceManager.RegisterHandler(new Texture2DHandler());
        ResourceManager.RegisterHandler(new MaterialHandler());

		ControlSchemeManager.Initialize();

		DreamJournalManager.Initialize();

		MapReader.MapScaleFactor = 1F;

		GameSettings.LoadSettings();

		Shader.SetGlobalFloat("_FogStep", 0.08F);
		Shader.SetGlobalFloat("AffineIntensity", 0.5F);
		
		PsxVram.Initialize();

		if (Application.isEditor)
		{
			// if we're running inside the editor, we want to have the mouse!
			GameSettings.SetCursorViewState(true);
		}

		// TODO
		//SaveGameManager.LoadGame();
	}
}
