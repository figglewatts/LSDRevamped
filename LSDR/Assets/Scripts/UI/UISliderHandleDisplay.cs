using System;
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
        public int DecimalPlaces = 0;

        public void ChangeTextToSliderValue(float value)
        {
            HandleText.text = Math.Round(value * ScaleFactor, DecimalPlaces) + PostFix;
        }

        public void Start()
        {
            _slider = GetComponent<Slider>();
            ChangeTextToSliderValue(_slider.value);
            _slider.onValueChanged.AddListener(ChangeTextToSliderValue);
        }
    }
}
