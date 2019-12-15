using System;
using LSDR.Dream;
using Torii.Console;
using Torii.Serialization;
using UnityEngine;

namespace LSDR.Game
{
    public class TestScript : MonoBehaviour
    {
        [Console]
        public string TestProperty { get; set; }

        [Console]
        public int TestVar;

        public bool SubtractiveFog;

        public int TextureSet;


        public void Update()
        {
            Shader.SetGlobalInt("_SubtractiveFog", SubtractiveFog ? 1 : 0);
            Shader.SetGlobalInt("_TextureSet", TextureSet);
        }
    }
}