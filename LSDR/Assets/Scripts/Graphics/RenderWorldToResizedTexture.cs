using UnityEngine;
using System.Collections;
using Game;
using UnityEngine.UI;
using UnityEngine.VR;

namespace Graphics
{
	public class RenderWorldToResizedTexture : MonoBehaviour
	{
		//how chunky to make the screen
		public int PixelSize = 4;
		public FilterMode FilterMode = FilterMode.Point;
		public Camera[] ImageCameras;
		public RawImage Image;

		private RenderTexture _tex;

		void Start()
		{
			if (GameSettings.VR) return;

			int width = GameSettings.UsePixelationShader ? Screen.width/PixelSize : Screen.width;
			int height = GameSettings.UsePixelationShader ? Screen.height/PixelSize : Screen.height;
		
			_tex = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
			_tex.filterMode = FilterMode;
			_tex.generateMips = false;
			_tex.Create();
			foreach (Camera c in ImageCameras) c.targetTexture = _tex;
		}

		void OnPostRender() { Image.texture = _tex; }
	}
}