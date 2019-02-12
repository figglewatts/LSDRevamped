using System.Reflection;
using Game;
using UnityEngine;
using InputManagement;
using IO;
using ResourceHandlers;
using Visual;
using ResourceManager = Torii.Resource.ResourceManager;

[assembly: AssemblyVersion("0.1.*")]
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

		// TODO
		//SaveGameManager.LoadGame();
	}
}
