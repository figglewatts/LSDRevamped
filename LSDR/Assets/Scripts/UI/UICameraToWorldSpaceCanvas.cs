using UnityEngine;
using System.Collections;

namespace UI
{
	/// <summary>
	/// Makes a worldspace canvas resize based on camera FOV by having it start as a camera canvas
	/// then switching it to worldspace when it's loaded
	/// </summary>
	[RequireComponent(typeof(Canvas))]
	public class UICameraToWorldSpaceCanvas : MonoBehaviour
	{
		private Canvas _canvas;

		void Awake() { _canvas = GetComponent<Canvas>(); }

		void Start() { _canvas.renderMode = RenderMode.WorldSpace; }
	}
}