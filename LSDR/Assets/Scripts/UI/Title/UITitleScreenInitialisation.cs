using System;
using LSDR.Dream;
using Torii.UI;
using UnityEngine;

namespace LSDR.UI.Title
{
    public class UITitleScreenInitialisation : MonoBehaviour
    {
        public GameObject Graph;
        public DreamSystem DreamSystem;
        public UIViewStack ViewStack;

        public void Start()
        {
            ShowTitleScreen();

            if (DreamSystem.ReturningFromDream)
            {
                DreamSystem.ReturningFromDream = false;
                ViewStack.PushWithoutTransition(Graph);
            }
        }

        public void ShowTitleScreen()
        {
            ToriiFader.Instance.FadeOut(Color.black, duration: 5, onFinish: null, initialAlpha: 1);
        }
    }
}
