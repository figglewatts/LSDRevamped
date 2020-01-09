using System;
using System.Collections;
using LSDR.Dream;
using Torii.Audio;
using Torii.Console;
using Torii.Serialization;
using Torii.Util;
using UnityEngine;

namespace LSDR.Game
{
    public class TestScript : MonoBehaviour
    {
        public void Start()
        {
            AudioClip clip = Torii.Resource.ResourceManager.Load<AudioClip>(
                PathUtil.Combine(Application.streamingAssetsPath,
                    "music/lsdr/Aidan Glad - Electro.ogg"));
            AudioPlayer.Instance.PlayClip(clip);
        }
    }
}