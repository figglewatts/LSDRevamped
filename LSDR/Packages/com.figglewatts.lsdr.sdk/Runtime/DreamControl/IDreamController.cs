using System.Collections.Generic;
using LSDR.SDK.Data;
using LSDR.SDK.Entities;
using UnityEngine;
using UnityEngine.Video;

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

        void StretchDream(float amount, float timeSeconds);

        void PlayVideo(VideoClip video, Color fadeInColor);
        void VideoFinished();

        void LogGraphContributionFromArea(int dynamicness, int upperness);
        void LogGraphContributionFromEntity(int dynamicness, int upperness, BaseEntity sourceEntity);

        void SetNextLinkDream(Dream dream, string spawnPointID = null);

        void SetCanControlPlayer(bool state);

        DreamJournal GetCurrentJournal();

        List<Dream> GetDreamsFromJournal();
    }
}
