using System;
using System.Collections;
using LSDR.Dream;
using LSDR.UI;
using Torii.Audio;
using Torii.Console;
using Torii.Serialization;
using Torii.UI;
using Torii.Util;
using UnityEngine;

namespace LSDR.Game
{
    public class TestScript : MonoBehaviour
    {
        private void Awake()
        {
            DevConsole.Register(this);
        }

        [Console]
        public void PrintStatement(string text)
        {
            Debug.Log(text);
        }
    }
}