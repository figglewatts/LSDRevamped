using System;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
	public static class AudioManager
	{
		private static readonly List<AudioSource> MusicPlayers = new List<AudioSource>();
		private static readonly List<AudioSource> SoundEffectPlayers = new List<AudioSource>();

		public static void ApplyVolume(float volume, AudioPlayerType type)
		{
			List<AudioSource> sourceList;
			GetPlayerListFromType(type, out sourceList);

			if (sourceList == null){ Debug.LogError("Could not set volume, invalid AudioPlayerType"); return; }
		
			foreach (AudioSource s in sourceList)
			{
				s.volume = volume;
			}
		}

		public static void RegisterAudioPlayer(AudioSource player, AudioPlayerType type)
		{
			List<AudioSource> sourceList;
			GetPlayerListFromType(type, out sourceList);
			sourceList.Add(player);
		}

		public static void DeregisterAudioPlayer(AudioSource player, AudioPlayerType type)
		{
			List<AudioSource> sourceList;
			GetPlayerListFromType(type, out sourceList);
			sourceList.Remove(player);
		}

		private static void GetPlayerListFromType(AudioPlayerType type, out List<AudioSource> list)
		{
			switch (type)
			{
				case AudioPlayerType.MUSIC:
				{
					list = MusicPlayers;
					break;
				}
				case AudioPlayerType.SFX:
				{
					list = SoundEffectPlayers;
					break;
				}
				default:
				{
					list = null;
					break;
				}
			}
		}
	}

	public enum AudioPlayerType
	{
		MUSIC,
		SFX
	}

	
}
