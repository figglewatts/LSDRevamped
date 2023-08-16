using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LSDR.SDK.Audio;
using LSDR.SDK.Util;
using Torii.Audio;
using Torii.Resource;
using UnityEngine;

namespace LSDR.Audio
{
    [CreateAssetMenu(menuName = "System/MusicSystem")]
    public class MusicSystem : ScriptableObject
    {
        public Action<SongAsset> OnSongChange;

        public SongAsset CurrentSong { get; protected set; }

        public AbstractSongLibrary CurrentSongLibrary
        {
            get => _usingOriginalSongs ? OriginalSongLibrary : _currentSongLibrary;
            set => _currentSongLibrary = value;
        }

        public OriginalSongLibrary OriginalSongLibrary;

        protected AbstractSongLibrary _currentSongLibrary;
        protected bool _usingOriginalSongs = false;
        protected SongStyle _songStyle = SongStyle.Standard;
        protected int _lastDayNumber = 1;

        public void UseOriginalSongs(bool useOriginalSongs)
        {
            // we want to change songs if we are playing, and if the value given here is different
            bool shouldChangeSong = MusicPlayer.Instance.IsPlaying && _usingOriginalSongs != useOriginalSongs;
            _usingOriginalSongs = useOriginalSongs;
            if (shouldChangeSong) NextSong(_lastDayNumber);
        }

        public void NextSong(int dayNumber)
        {
            CurrentSong = CurrentSongLibrary.GetSong(_songStyle, dayNumber);
            _lastDayNumber = dayNumber;
            MusicPlayer.Instance.PlaySong(CurrentSong);
            OnSongChange?.Invoke(CurrentSong);
        }

        public void StopSong()
        {
            MusicPlayer.Instance.StopSong();
            CurrentSong = null;
            OnSongChange?.Invoke(null);
        }

        public void SetSongStyle(SongStyle style)
        {
            _songStyle = style;
            if (MusicPlayer.Instance.IsPlaying) NextSong(_lastDayNumber);
        }
    }
}
