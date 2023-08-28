using System.Linq;
using LSDR.SDK.Audio;
using Torii.Resource;
using UnityEngine.Audio;

namespace LSDR.Audio
{
    public class MixerGroupProvider : IMixerGroupProvider
    {
        protected const string MIXER_PATH = "Mixers/MasterMixer";

        public AudioMixerGroup GetMixerGroup(string group)
        {
            AudioMixer mixer = ResourceManager.UnityLoad<AudioMixer>(MIXER_PATH);
            return mixer.FindMatchingGroups(group).FirstOrDefault();
        }
    }
}
