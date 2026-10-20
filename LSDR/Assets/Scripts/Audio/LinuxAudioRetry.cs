using UnityEngine;

namespace LSDR.Audio
{
    // on some linux setups the audio device isn't ready when the player starts, which leaves the whole game silent,
    // so restart the audio output once the splash screen is done, before anything has started playing
    public static class LinuxAudioRetry
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void retry()
        {
            if (Application.platform != RuntimePlatform.LinuxPlayer) return;

            Debug.Log("Restarting audio output");
            if (!AudioSettings.Reset(AudioSettings.GetConfiguration()))
            {
                Debug.LogWarning("Unable to restart audio output");
            }
        }
    }
}