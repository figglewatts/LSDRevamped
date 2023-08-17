using System.Collections;
using LSDR.Game;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace LSDR.UI.Title
{
    public class UIUpdateChecker : MonoBehaviour
    {
        private const string LATEST_VERSION_URL =
            "https://itch.io/api/1/x/wharf/latest?target=figglewatts/lsd-revamped&channel_name=windows";
        public Text NewVersionText;

        public void Start() { StartCoroutine(checkIsLatestVersion()); }

        private IEnumerator checkIsLatestVersion()
        {
            using (UnityWebRequest req = UnityWebRequest.Get(LATEST_VERSION_URL))
            {
                yield return req.SendWebRequest();

                if (req.isNetworkError)
                {
                    Debug.LogWarning($"Unable to check for latest version: {req.error}");
                }
                else
                {
                    VersionResponse version = JsonConvert.DeserializeObject<VersionResponse>(req.downloadHandler.text);
                    if (!string.IsNullOrEmpty(version.latest) && !isLatestVersion(version.latest))
                    {
                        NewVersionText.text =
                            $"A new version is available! Go to itch.io to download version {version.latest}";
                    }
                }
            }
        }

        private bool isLatestVersion(string latestVersion)
        {
            string buildNumber = typeof(GameLoadSystem).Assembly.GetName().Version.ToString();
            return latestVersion.Equals(buildNumber);
        }

        private class VersionResponse
        {
            public string latest { get; }
        }
    }
}
