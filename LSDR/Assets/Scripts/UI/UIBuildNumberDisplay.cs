using UnityEngine;
using UnityEngine.UI;

namespace LSDR.UI
{
	/// <summary>
	/// Display the build number on text at the top of the screen.
	/// </summary>
    public class UIBuildNumberDisplay : MonoBehaviour
	{
		public Text BuildNumberText;

        void Start()
        {
	        BuildNumberText.text = getVersionString();
        }

        protected string getVersionString()
        {
	        var versionString = typeof(UIBuildNumberDisplay).Assembly.GetName().Version.ToString();
	        var components = versionString.Split(new [] {'.'}, 4);
	        var major = components[0];
	        var minor = components[1];
	        var build = components[2];
	        var rev = components[3];
	        return $"{major}.{minor}.{build} (id. {rev})";
        }
    }
}
