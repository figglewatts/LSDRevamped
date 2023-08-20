using System;
using System.Collections.Generic;
using UnityEngine;

namespace LSDR.SDK.Audio
{
    [CreateAssetMenu(menuName = "LSDR SDK/Original Song Library")]
    public class OriginalSongLibrary : AbstractSongLibrary
    {
        public List<DreamOriginalSongs> OriginalDreams;

        public int DreamNumber
        {
            get => _dreamNumber;
            set => _dreamNumber = value % OriginalDreams.Count;
        }

        protected int _dreamNumber = 0;

        public override SongAsset GetSong(SongStyle style, int songNumber)
        {
            Debug.Log(style);
            var dreamSongs = OriginalDreams[DreamNumber];
            switch (style)
            {
                case SongStyle.Standard:
                    return getSongFromNumber(dreamSongs.StandardSongs, songNumber);
                case SongStyle.Lovely:
                    return getSongFromNumber(dreamSongs.LovelySongs, songNumber);
                case SongStyle.Electro:
                    return getSongFromNumber(dreamSongs.ElectroSongs, songNumber);
                case SongStyle.Ethnova:
                    return getSongFromNumber(dreamSongs.EthnovaSongs, songNumber);
                case SongStyle.Cartoon:
                    return getSongFromNumber(dreamSongs.CartoonSongs, songNumber);
                case SongStyle.Human:
                    return getSongFromNumber(dreamSongs.HumanSongs, songNumber);
                case SongStyle.Ambient:
                    return getSongFromNumber(dreamSongs.AmbientSongs, songNumber);
                default:
                    throw new ArgumentOutOfRangeException(nameof(style), style, null);
            }
        }

        protected SongAsset getSongFromNumber(SongListAsset songList, int songNumber)
        {
            return songList.Songs[songNumber % songList.Songs.Count];
        }
    }
}
