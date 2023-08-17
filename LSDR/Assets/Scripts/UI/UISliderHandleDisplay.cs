using System;
using Torii.UI;
using UnityEngine;

namespace LSDR.UI
{
    /// <summary>
    ///     Display a value on the handle of a UI slider.
    /// </summary>
    public class UISliderHandleDisplay : MonoBehaviour
    {
        public Text HandleText;

        public float ScaleFactor = 100;
        public string PostFix = "%";
        public int DecimalPlaces;
        private Slider _slider;

        public void Start()
        {
            _slider = GetComponent<Slider>();
            ChangeTextToSliderValue(_slider.value);
            _slider.onValueChanged.AddListener(ChangeTextToSliderValue);
        }

        public void ChangeTextToSliderValue(float value)
        {
            HandleText.text = Math.Round(value * ScaleFactor, DecimalPlaces) + PostFix;
        }
    }
}
