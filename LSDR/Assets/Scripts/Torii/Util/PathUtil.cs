using System;
using System.IO;

namespace Torii.Util
{
    /// <summary>
    ///     Various utility methods for paths.
    /// </summary>
    public static class PathUtil
    {
        /// <summary>
        ///     Combines two elements into a path. Converts back slashes in path to forward slashes.
        /// </summary>
        /// <param name="a">Element A</param>
        /// <param name="b">Element B</param>
        /// <returns>The combined path</returns>
        public static string Combine(string a, string b)
        {
            if (b.StartsWith("\\") || b.StartsWith("/"))
            {
                b = b.Substring(startIndex: 1);
            }

            return Path.Combine(a, b).Replace(oldChar: '\\', newChar: '/');
        }

        /// <summary>
        ///     Combines any number of path parameters. Converts back slashes in path to forward slashes.
        /// </summary>
        /// <param name="componentStrings">Any number of path elements to combine</param>
        /// <returns>The combined path.</returns>
        public static string Combine(params string[] componentStrings)
        {
            string path = componentStrings[0];
            for (int i = 1; i < componentStrings.Length; i++)
            {
                path = Combine(path, componentStrings[i]);
            }

            return path.Replace(oldChar: '\\', newChar: '/');
        }

        public static string SanitiseFileName(string fileName)
        {
            char[] invalids = Path.GetInvalidFileNameChars();
            return string.Join("_", fileName.Split(invalids, StringSplitOptions.RemoveEmptyEntries)).TrimEnd('.');
        }
    }
}
