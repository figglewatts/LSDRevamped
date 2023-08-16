using System;
using LSDR.Audio;
using LSDR.Dream;
using LSDR.Game;
using LSDR.SDK.Audio;
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
            MusicSystem.OnSongChange += UpdateSongText;
            UpdateSongText(MusicSystem.CurrentSong);
        }

        public void OnDisable()
        {
            MusicSystem.OnSongChange -= UpdateSongText;
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

        public void UpdateSongText(SongAsset currentSong)
        {
            if (currentSong == null)
            {
                SongNameTextElement.text = "No song playing";
                SongArtistTextElement.text = "";
                return;
            }

            SongNameTextElement.text = MusicSystem.CurrentSong.Name;
            SongArtistTextElement.text = MusicSystem.CurrentSong.Author;
        }
    }
}
