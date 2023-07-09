using LSDR.SDK.Data;
using UnityEngine;

namespace LSDR.SDK.DreamControl
{
    public interface IDreamController
    {
        void Transition(Color fadeCol,
            Dream dream = null,
            bool playSound = true,
            bool lockInput = false,
            string spawnPointID = null);

        void Transition(Dream dream = null,
            bool playSound = true,
            bool lockInput = false,
            string spawnPointID = null);

        void EndDream(bool fromFall = false);

        void LogGraphContributionFromArea(int dynamicness, int upperness);
        void LogGraphContributionFromEntity(int dynamicness, int upperness);
    }
}
