using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Torii.Util
{
    /// <summary>
    /// Various utility methods for paths.
    /// </summary>
    public static class PathUtil
    {
        /// <summary>
        /// Combines two elements into a path. Converts back slashes in path to forward slashes.
        /// </summary>
        /// <param name="a">Element A</param>
        /// <param name="b">Element B</param>
        /// <returns>The combined path</returns>
        public static string Combine(string a, string b)
        {
            if (b.StartsWith("\\") || b.StartsWith("/"))
            {
                b = b.Substring(1);
            }
            return Path.Combine(a, b).Replace('\\', '/');
        }

        /// <summary>
        /// Combines any number of path parameters. Converts back slashes in path to forward slashes.
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
            return path.Replace('\\', '/');
        }
    }
}