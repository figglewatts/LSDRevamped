using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using LSDR.SDK.Audio;
using UnityEditor;
using UnityEngine;

namespace LSDR.Util
{
    public static class ContextTools
    {
        [MenuItem("Assets/LSDR/Hook up original songs")]
        public static void HookUpOriginalSongs()
        {
            const string stgRegex = @"^Assets\/Original\/(.*)\/LoadedTracks$";
            const string styleRegex = @"^BG.-(.*)$";

            var projectPath = getProjectWindowActiveFolderPath();
            var stgNameMatch = Regex.Match(projectPath, stgRegex);
            if (stgNameMatch.Length <= 0)
            {
                Debug.LogError("You need to be in the LoadedTracks folder of STGXX to use this!");
                return;
            }
            string stgName = stgNameMatch.Groups[1].Value;

            Dictionary<string, SongListAsset> styleLists = new Dictionary<string, SongListAsset>()
            {
                { "AMBIENT", AssetDatabase.LoadAssetAtPath<SongListAsset>($"{projectPath}/{stgName}Ambient.asset") },
                { "CARTOON", AssetDatabase.LoadAssetAtPath<SongListAsset>($"{projectPath}/{stgName}Cartoon.asset") },
                { "ELECTRO", AssetDatabase.LoadAssetAtPath<SongListAsset>($"{projectPath}/{stgName}Electro.asset") },
                { "ETHNOVA", AssetDatabase.LoadAssetAtPath<SongListAsset>($"{projectPath}/{stgName}Ethnova.asset") },
                { "HUMAN", AssetDatabase.LoadAssetAtPath<SongListAsset>($"{projectPath}/{stgName}Human.asset") },
                { "LOVELY", AssetDatabase.LoadAssetAtPath<SongListAsset>($"{projectPath}/{stgName}Lovely.asset") },
                { "STANDERD", AssetDatabase.LoadAssetAtPath<SongListAsset>($"{projectPath}/{stgName}Standerd.asset") },
            };
            foreach (var kvp in styleLists)
            {
                kvp.Value.Songs.Clear();
            }

            foreach (var songAsset in Directory.GetFiles(projectPath, "*.wav", SearchOption.AllDirectories))
            {
                var songFileName = Path.GetFileName(songAsset);
                var songFileNameWithoutExtension = Path.GetFileNameWithoutExtension(songAsset);

                var styleMatch = Regex.Match(songFileNameWithoutExtension, styleRegex);
                if (styleMatch.Length <= 0)
                {
                    Debug.LogError("Invalid song name in file structure, path: " + songAsset);
                    return;
                }
                var songStyle = styleMatch.Groups[1].Value;

                var songDirectory = Path.GetDirectoryName(songAsset);
                var songAssetPath = $"{songDirectory}/{songFileNameWithoutExtension}Song.asset";
                var existingAsset = AssetDatabase.LoadAssetAtPath<SongAsset>(songAssetPath);

                // create song asset for song if not exists
                if (existingAsset == null)
                {
                    existingAsset = ScriptableObject.CreateInstance<SongAsset>();
                    AssetDatabase.CreateAsset(existingAsset, songAssetPath);
                }
                AudioClip clipAsset = AssetDatabase.LoadAssetAtPath<AudioClip>(songAsset);
                existingAsset.Clip = clipAsset;
                existingAsset.Author = "OutSide Directors Company";
                existingAsset.Name = songFileNameWithoutExtension;
                styleLists[songStyle].Songs.Add(existingAsset);
                EditorUtility.SetDirty(styleLists[songStyle]);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static string getProjectWindowActiveFolderPath()
        {
            Type projectWindowUtilType = typeof(ProjectWindowUtil);
            MethodInfo getActiveFolderPath =
                projectWindowUtilType.GetMethod("GetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);
            object obj = getActiveFolderPath!.Invoke(null, Array.Empty<object>());
            return obj.ToString();
        }
    }
}
