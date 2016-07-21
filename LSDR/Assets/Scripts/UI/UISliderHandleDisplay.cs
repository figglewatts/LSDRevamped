using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace UI
{
	public class UISliderHandleDisplay : MonoBehaviour
	{
		public Text TextElement;
		public float ScaleFactor = 100;
		public string PostFix = "%";

		public void ChangeTextToSliderValue(float value) { TextElement.text = Mathf.Round(value*ScaleFactor) + PostFix; }
	}
}