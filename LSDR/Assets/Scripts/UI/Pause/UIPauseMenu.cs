﻿using LSDR.Audio;
using LSDR.Dream;
using LSDR.Game;
using UnityEngine;
using UnityEngine.UI;

namespace LSDR.UI
{
    /// <summary>
    ///     The pause menu of the game.
    /// </summary>
    public class UIPauseMenu : MonoBehaviour
    {
        public Text SongNameTextElement;
        public Text SongArtistTextElement;

        public DreamSystem DreamSystem;
        public MusicSystem MusicSystem;
        public PauseSystem PauseSystem;

        public void OnEnable()
        {
            UpdateSongText();
        }

        public void ReturnToMenu()
        {
            PauseSystem.TogglePause();
            DreamSystem.ForceEndDream();
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        public void UpdateSongText()
        {
            SongNameTextElement.text = MusicSystem.CurrentSong;
            SongArtistTextElement.text = MusicSystem.CurrentArtist;
        }
    }
}
