using System;
using LSDR.Dream;
using UnityEngine;

namespace LSDR.Game
{
    public class LoadModScript : MonoBehaviour
    {
        public DreamSystem DreamSystem;
        public GameLoadSystem GameLoadSystem;

        public void Start()
        {
            if (GameLoadSystem.GameLoaded) LoadMod();
        }

        public void LoadMod()
        {
            DreamSystem.BeginDream();
        }
    }
}
