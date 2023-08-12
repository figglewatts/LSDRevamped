﻿using UnityEngine;
using UnityEngine.UI;

namespace LSDR.UI
{
    /// <summary>
    ///     An output row in the console.
    /// </summary>
    public class UIConsoleOutputRow : MonoBehaviour
    {
        public Image IconImage;
        public Text OutputText;
        public Color IconColor;
        public Color TextColor;
        public string OutputMessage;
        public Sprite IconSprite;

        public void Start()
        {
            IconImage.sprite = IconSprite;
            IconImage.color = IconColor;
            OutputText.color = TextColor;
            OutputText.text = OutputMessage;
        }
    }
}
