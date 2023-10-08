using System.Collections.Generic;
using LSDR.SDK.Data;
using UnityEngine;

namespace LSDR.SDK.DreamControl
{
    public interface IDreamController
    {
        public bool InDream { get; }
        public int CurrentDay { get; }

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

        void SetNextLinkDream(Dream dream, string spawnPointID = null);

        void SetCanControlPlayer(bool state);

        DreamJournal GetCurrentJournal();

        List<Dream> GetDreamsFromJournal();
    }
}
