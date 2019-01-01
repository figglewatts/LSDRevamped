using System.Reflection;
using Game;
using UnityEngine;
using InputManagement;
using IO;
using Types;
using UI;
using Util;

[assembly: AssemblyVersion("0.1.*")]
public class GameScript : MonoBehaviour
{
	void Awake()
	{
		// do game startup stuff here

		ControlSchemeManager.Initialize();

		DreamJournalManager.Initialize();

		MapReader.MapScaleFactor = 1F;

		GameSettings.LoadSettings();

		Shader.SetGlobalFloat("_FogStep", 0.08F);
		Shader.SetGlobalFloat("AffineIntensity", 0.5F);

		SaveGameManager.LoadGame();
	}
}
