using LSDR.SDK.Data;
using LSDR.SDK.DreamControl;
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

        protected override Color _editorColour { get; } = new Color(1, 0.6f, 0);

        protected override void onTrigger(Collider player)
        {
            string forcedSpawnPoint = string.IsNullOrWhiteSpace(SpawnPointEntityID) ? null : SpawnPointEntityID;

            if (!ForceFadeColor)
            {
                DreamControlManager.Managed.Transition(Linked, PlayLinkSound, LockInput, forcedSpawnPoint);
            }
            else
            {
                DreamControlManager.Managed.Transition(ForcedLinkColor, Linked, PlayLinkSound, LockInput,
                    forcedSpawnPoint);
            }
        }
    }
}
