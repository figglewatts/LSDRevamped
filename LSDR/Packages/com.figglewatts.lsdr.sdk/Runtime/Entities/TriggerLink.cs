using LSDR.SDK.Data;
using LSDR.SDK.DreamControl;
using LSDR.SDK.Util;
using UnityEngine;

namespace LSDR.SDK.Entities
{
    public class TriggerLink : BaseTrigger
    {
        public Dream Linked;

        public Color ForcedLinkColor;
        public string SpawnPointEntityID;

        public bool ForceFadeColor;
        public bool PlayLinkSound;
        public bool LockInput;

        protected override void onTrigger(Collider player)
        {
            Color linkColor = ForceFadeColor ? ForcedLinkColor : RandUtil.RandColor();
            string forcedSpawnPoint = string.IsNullOrWhiteSpace(SpawnPointEntityID) ? null : SpawnPointEntityID;
            DreamControlManager.Managed.Transition(linkColor, Linked, PlayLinkSound, LockInput, forcedSpawnPoint);
        }
    }
}
