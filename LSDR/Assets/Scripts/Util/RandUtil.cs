using System;
using System.IO;
using UnityEngine;
using Random = System.Random;

namespace LSDR.Util
{
	/// <summary>
	/// Utility functions for generating random numbers.
	/// TODO: Refactor RandUtil to not be static, as will help for determinism of random numbers?
	/// </summary>
	public static class RandUtil
	{
		/// <summary>
		/// The current seed of the random number generator.
		/// </summary>
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

		/// <summary>
		/// Refresh the current random generator seed.
		/// </summary>
		public static void RefreshSeed()
		{
			CurrentSeed = (int) DateTime.Now.Ticks & 0x0000FFFF;
            _rand = new Random(CurrentSeed);
		}

		/// <summary>
		/// Set the random generator to a given seed. Used for determinism.
		/// </summary>
		/// <param name="seed">The seed to set it to.</param>
		public static void SetSeed(int seed)
		{
			_rand = new Random(seed);
		}

		/// <summary>
		/// Get a non-negative random integer.
		/// </summary>
		/// <returns>A non-negative random integer.</returns>
		public static int Int() { return _rand.Next(); }
		
		/// <summary>
		/// Get a random non-negative integer with exclusive maximum value.
		/// </summary>
		/// <param name="max">The exlusive max value.</param>
		/// <returns>A random non-negative integer.</returns>
		public static int Int(int max) { return _rand.Next(max); }
		
		/// <summary>
		/// Get a random non-negative integer between a min and max value.
		/// </summary>
		/// <param name="min">The inclusive min bound.</param>
		/// <param name="max">The exlusive max bound.</param>
		/// <returns>The random integer.</returns>
		public static int Int(int min, int max) { return _rand.Next(min, max); }

		/// <summary>
		/// Get a random float x, where 0.0 &lt;= x &lt; 1.0
		/// </summary>
		/// <returns>The random float.</returns>
		public static float Float() { return (float)_rand.NextDouble(); }
		
		/// <summary>
		/// Get a random float between given bounds.
		/// </summary>
		/// <param name="min">Inclusive min bound.</param>
		/// <param name="max">Exclusive max bound.</param>
		/// <returns>The random float.</returns>
		public static float Float(float min, float max) { return (min + Float()*(max - min)); }
		
		/// <summary>
		/// Get a random float with a max possible value (exclusive).
		/// </summary>
		/// <param name="max">The exclusive max bound.</param>
		/// <returns>The random float.</returns>
		public static float Float(float max) { return Float(0, max); }

		/// <summary>
		/// Generate a random color.
		/// TODO: refactor RandUtil.RandColor() when making colors better
		/// </summary>
		/// <returns>A random color.</returns>
		public static Color RandColor() { return _randomColors[Int(_randomColors.Length)]; }

		public static string RandomLevelFromDir(string levelDir)
		{
			// TODO: refactor RandUtil.RandomLevelFromDir in DreamDirector refactor
			string[] filesInDir = Directory.GetFiles(IOUtil.PathCombine(Application.streamingAssetsPath, "levels", levelDir), "*.tmap");
			return IOUtil.PathCombine("levels", levelDir, filesInDir[Int(filesInDir.Length)]);
		}

		/// <summary>
		/// Choose a random element from an array.
		/// </summary>
		/// <param name="array">The array.</param>
		/// <typeparam name="T">The type of the object contained within the array.</typeparam>
		/// <returns>The random choice.</returns>
		public static T RandomArrayElement<T>(T[] array) { return array[Int(array.Length)]; }
	}
}
