using UnityEngine.Audio;

namespace LSDR.SDK.Audio
{
    public interface IMixerGroupProvider
    {
        public AudioMixerGroup GetMixerGroup(string group);
    }
}
