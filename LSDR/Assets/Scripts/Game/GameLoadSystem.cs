using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using LSDR.InputManagement;
using LSDR.IO;
using LSDR.IO.ResourceHandlers;
using LSDR.Visual;
using Torii.Event;
using Torii.Pooling;
using TResourceManager = Torii.Resource.ResourceManager; // TODO: change when old ResourceManager removed

[assembly: AssemblyVersion("0.1.*")]
namespace LSDR.Game
{
	/// <summary>
	/// GameLoadSystem is a system that contains code for initialising the game.
	/// </summary>
	[CreateAssetMenu(menuName="System/GameLoadSystem")]
	public class GameLoadSystem : ScriptableObject
	{
		public ToriiEvent OnGameDataLoaded;
		public PrefabPool LBDTilePool;
		
		public IEnumerator LoadGameCoroutine()
		{
			// do game startup stuff here

			TResourceManager.RegisterHandler(new LBDHandler());
			TResourceManager.RegisterHandler(new TIXHandler());
			TResourceManager.RegisterHandler(new Texture2DHandler());
			TResourceManager.RegisterHandler(new MaterialHandler());

			ControlSchemeManager.Initialize();

			MapReader.MapScaleFactor = 1F;

			Shader.SetGlobalFloat("_FogStep", 0.08F);
			Shader.SetGlobalFloat("AffineIntensity", 0.5F);

			PsxVram.Initialize();

			if (Application.isEditor)
			{
				// if we're running inside the editor, we want to have the mouse!
				//GameSettings.SetCursorViewState(true);
			}

			yield return LBDTilePool.InitialiseCoroutine();

			// TODO
			//SaveGameManager.LoadGame();
			
			OnGameDataLoaded.Raise();
		}
	}
}