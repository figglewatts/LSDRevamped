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

        void EndDream(bool fromFall = false);
    }
}