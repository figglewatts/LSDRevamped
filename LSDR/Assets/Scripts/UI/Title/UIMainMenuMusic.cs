using System;
using LSDR.Game;
using UnityEngine;

namespace LSDR.UI.Title
{
    public class UIMainMenuMusic : MonoBehaviour
    {
        public AudioSource source;
        public AudioClip NormalMusic;
        public AudioClip KanjiMusic;
        public AudioClip DownerMusic;
        public AudioClip UpperMusic;
        public GameSaveSystem GameSave;

        public void Start()
        {
            PlayMusic();
        }

        public void PlayMusic()
        {
            int dayNumMod = GameSave.CurrentJournalSave.DayNumber % 41;
            if (dayNumMod <= 10)
                source.clip = NormalMusic;
            else if (dayNumMod <= 20)
                source.clip = KanjiMusic;
            else if (dayNumMod <= 30)
                source.clip = DownerMusic;
            else if (dayNumMod <= 40) source.clip = UpperMusic;

            source.loop = true;
            source.Play();
        }
    }
}
