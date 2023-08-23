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

        public string SpawnPointEntityIDWithNull =>
            string.IsNullOrWhiteSpace(SpawnPointEntityID) ? null : SpawnPointEntityID;

        public bool ForceFadeColor;
        public bool PlayLinkSound;
        public bool LockInput;
        public bool AffectsLinksWithin;

        protected override Color _editorColour { get; } = new Color(r: 1, g: 0.6f, b: 0);

        protected override void onTrigger(Collider player)
        {
            string forcedSpawnPoint = SpawnPointEntityIDWithNull;

            if (AffectsLinksWithin)
            {
                DreamControlManager.Managed.SetNextLinkDream(Linked, forcedSpawnPoint);
                return;
            }

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

        protected override void onTriggerExit(Collider player)
        {
            if (AffectsLinksWithin)
            {
                DreamControlManager.Managed.SetNextLinkDream(null, null);
            }
        }
    }
}
