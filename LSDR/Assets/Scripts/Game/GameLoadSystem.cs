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
using Torii.UI;
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

		[NonSerialized]
		public static bool GameLoaded = false;

		public IEnumerator LoadGameCoroutine()
		{
			// do game startup stuff here

			TResourceManager.RegisterHandler(new LBDHandler());
			TResourceManager.RegisterHandler(new TIXHandler());
			TResourceManager.RegisterHandler(new Texture2DHandler());
			TResourceManager.RegisterHandler(new MOMHandler());
			TResourceManager.RegisterHandler(new ToriiAudioClipHandler());

			Screenshotter.Instance.Init();
			
			// set the sort order for the fader so the version text appears on top during fades
			ToriiFader.Instance.SetSortOrder(0);

			Shader.SetGlobalFloat("_FogStep", 0.08F);
			Shader.SetGlobalFloat("AffineIntensity", 0.5F);

			// okay, it's a fake loading screen LOL
			// it used to be real but it's been optimised away and I liked the animation too much
			if (!Application.isEditor) 
				yield return new WaitForSeconds(3);
			else 
				yield return null;

			GameLoaded = true;

			OnGameDataLoaded.Raise();
		}
	}
}