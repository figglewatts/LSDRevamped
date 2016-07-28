using System;
using System.IO;
using UnityEngine;
using Random = System.Random;

namespace Util
{
	public static class RandUtil
	{
		public static int CurrentSeed { get; private set; }
	
		private static Random _rand;

		private static readonly Color[] _randomColors = new[]
		{
			Color.white,
			Color.red,
			Color.yellow,
			Color.blue,
			Color.cyan,
			Color.green,
			Color.magenta
		};

		static RandUtil() { RefreshSeed(); }

		public static void RefreshSeed()
		{
			CurrentSeed = (int) DateTime.Now.Ticks & 0x0000FFFF;
            _rand = new Random(CurrentSeed);
		}

		public static void SetSeed(int seed)
		{
			_rand = new Random(seed);
		}

		public static int Int() { return _rand.Next(); }
		public static int Int(int max) { return _rand.Next(max); }
		public static int Int(int min, int max) { return _rand.Next(min, max); }

		public static float Float() { return (float)_rand.NextDouble(); }
		public static float Float(float min, float max) { return (min + Float()*(max - min)); }
		public static float Float(float max) { return Float(0, max); }

		public static Color RandColor() { return _randomColors[Int(_randomColors.Length)]; }

		public static string RandomLevelFromDir(string levelDir)
		{
			string[] filesInDir = Directory.GetFiles(IOUtil.PathCombine(Application.dataPath, "levels", levelDir), "*.tmap");
			return IOUtil.PathCombine("levels", levelDir, filesInDir[Int(filesInDir.Length)]);
		}

		public static T RandomArrayElement<T>(T[] array) { return array[Int(array.Length)]; }
	}
}
