using System;
using System.Collections;
using LSDR.Dream;
using Torii.Console;
using Torii.Serialization;
using UnityEngine;

namespace LSDR.Game
{
    public class TestScript : MonoBehaviour
    {
        public DreamSystem DreamSystem;

        public void Start() { StartCoroutine(EndAfterTime()); }

        private IEnumerator EndAfterTime()
        {
            yield return new WaitForSeconds(15f);
            
            DreamSystem.EndDream();
        }
    }
}