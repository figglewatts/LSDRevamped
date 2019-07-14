using Game;
using UnityEngine;
using UnityEngine.UI;

namespace Visual
{
	// TODO: is RenderWorldToResizedTexture used?
	public class RenderWorldToResizedTexture : MonoBehaviour
	{
		// how chunky to make the screen
		public int PixelSize = 4;
		public FilterMode FilterMode = FilterMode.Point;
		public Camera[] ImageCameras;
		public RawImage Image;

		private RenderTexture _tex;

		void Start()
		{
			if (GameSettings.VR) return;

			int width = GameSettings.CurrentSettings.UsePixelationShader ? Screen.width/PixelSize : Screen.width;
			int height = GameSettings.CurrentSettings.UsePixelationShader ? Screen.height/PixelSize : Screen.height;
		
			_tex = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
			_tex.filterMode = FilterMode;
			_tex.autoGenerateMips = false;
			_tex.Create();
			foreach (Camera c in ImageCameras) c.targetTexture = _tex;
		}

		void OnPostRender() { Image.texture = _tex; }
	}
}