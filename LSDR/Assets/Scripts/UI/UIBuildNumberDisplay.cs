using UnityEngine;
using UnityEngine.UI;

namespace LSDR.UI
{
    /// <summary>
    ///     Display the build number on text at the top of the screen.
    /// </summary>
    public class UIBuildNumberDisplay : MonoBehaviour
    {
        public Text BuildNumberText;
        public bool PreRelease = false;

        private void Start() { BuildNumberText.text = getVersionString(); }

        protected string getVersionString()
        {
            if (PreRelease)
            {
                return $"{Application.version} - PRERELEASE, anything could go wrong!";
            }
            if (Application.isEditor)
            {
                return $"{Application.version}-editor";
            }
            return Application.version;
        }
    }
}
