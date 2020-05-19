using System;
using UnityEngine;

namespace LSDR.Game
{
    public class HideIfGameLoaded : MonoBehaviour
    {
        public void Start()
        {
            gameObject.SetActive(!GameLoadSystem.GameLoaded);
        }
    }
}
