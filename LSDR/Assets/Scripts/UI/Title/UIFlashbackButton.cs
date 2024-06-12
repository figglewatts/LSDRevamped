using System;
using LSDR.Dream;
using LSDR.Game;
using UnityEngine;
using UnityEngine.UI;

namespace LSDR.UI.Title
{
    public class UIFlashbackButton : MonoBehaviour
    {
        public DreamSystem DreamSystem;
        public Button FlashbackButton;

        public void Start()
        {
            FlashbackButton.interactable = DreamSystem.CanFlashback;
        }

        public void OnEnable()
        {
            FlashbackButton.interactable = DreamSystem.CanFlashback;
        }
    }
}
