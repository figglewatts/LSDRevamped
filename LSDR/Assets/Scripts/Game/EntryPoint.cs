using System;
using Torii.Util;
using UnityEngine;

namespace LSDR.Game
{
    public class EntryPoint : MonoSingleton<EntryPoint>
    {
        public GameObject GamePrefab;

        public override void Init() { Instantiate(GamePrefab); }
    }
}
