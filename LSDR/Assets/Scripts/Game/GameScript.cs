using System.Collections;
using System.Reflection;
using UnityEngine;
using LSDR.InputManagement;
using LSDR.IO;
using LSDR.IO.ResourceHandlers;
using LSDR.Visual;
using Torii.Pooling;
using TResourceManager = Torii.Resource.ResourceManager; // TODO: change when old ResourceManager removed

[assembly: AssemblyVersion("0.1.*")]
namespace LSDR.Game
{
	/// <summary>
	/// GameScript is essentially the entrypoint to the game. All initialisation stuff is done here.
	/// It will be executed before MOST scripts.
	/// </summary>
	public class GameScript : MonoBehaviour
	{
		public static IEnumerator InitializeGame()
		{
			// do game startup stuff here
			
			GameSettings.Initialize();

			TResourceManager.RegisterHandler(new LBDHandler());
			TResourceManager.RegisterHandler(new TIXHandler());
			TResourceManager.RegisterHandler(new Texture2DHandler());
			TResourceManager.RegisterHandler(new MaterialHandler());

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
			
			var lbdTilePrefab = Resources.Load<GameObject>("Prefabs/Entities/LBDTile").GetComponent<PoolItem>();
			yield return PrefabPool.CreateCoroutine("LBDTilePool", lbdTilePrefab, LBDReader.MAX_POSSIBLE_TILES, true);

			// TODO
			//SaveGameManager.LoadGame();
		}
	}
}