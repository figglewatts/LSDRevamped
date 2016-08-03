using System;
using System.IO;
using UnityEngine;
using Util;

namespace AutoUpdate
{
	public static class BuildNumber
	{
		/// <summary>
		/// Reads 'version.txt' on the game's root dir to ascertain the client build number.
		/// </summary>
		public static int Get()
		{
			if (Application.platform == RuntimePlatform.WindowsEditor)
			{
				return -1;
			}

			try
			{
				using (StreamReader r = new StreamReader(IOUtil.PathCombine(Application.dataPath, "../", "version.txt")))
				{
					return int.Parse(r.ReadLine());
				}
			}
			catch (Exception e)
			{
				Debug.LogException(e);
				return -1;
			}
		}
	}
}