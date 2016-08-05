using UnityEngine;
using System.Collections;
using Game;
using UnityEngine.UI;

namespace Graphics
{
	public class RenderWorldToResizedTexture : MonoBehaviour
	{
		//how chunky to make the screen
		public int PixelSize = 4;
		public FilterMode FilterMode = FilterMode.Point;
		public Camera ImageCamera;
		public RawImage Image;

		private RenderTexture _tex;

		void Start()
		{
			int width = GameSettings.UsePixelationShader ? Screen.width/PixelSize : Screen.width;
			int height = GameSettings.UsePixelationShader ? Screen.height/PixelSize : Screen.height;
		
			_tex = new RenderTexture(width, height, 24);
			_tex.filterMode = FilterMode;
			_tex.generateMips = false;
			_tex.Create();
			ImageCamera.targetTexture = _tex;
		}

		void OnPostRender() { Image.texture = _tex; }
	}
}