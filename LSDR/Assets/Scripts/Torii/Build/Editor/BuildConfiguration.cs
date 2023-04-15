using System.IO;
using UnityEditor;
using UnityEngine;

namespace Torii.Build
{
    [CreateAssetMenu(menuName = "Torii/BuildConfiguration")]
    public class BuildConfiguration : ScriptableObject
    {
        public bool Enabled;
        public string Name;

        public BuildTarget Target;
        public BuildOptions BuildOptions;

        public string GetOutputPath(BuildSettings settings)
        {
            return Path.Combine(settings.OutputPath, Target.ToString(), settings.OutputName);
        }
    }
}
