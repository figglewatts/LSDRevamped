using UnityEngine;
using Torii.UI;

namespace LSDR.UI
{
	/// <summary>
	/// Display a value on the handle of a UI slider.
	/// </summary>
    public class UISliderHandleDisplay : MonoBehaviour
    {
        private Slider _slider;

        public Text HandleText;

        public float ScaleFactor = 100;
        public string PostFix = "%";

        public void ChangeTextToSliderValue(float value) { HandleText.text = Mathf.Round(value * ScaleFactor) + PostFix; }

        public void Start()
        {
            _slider = GetComponent<Slider>();
            ChangeTextToSliderValue(_slider.value);
            _slider.onValueChanged.AddListener(ChangeTextToSliderValue);
        }
	}
}