using LSDR.SDK.DreamControl;
using UnityEngine;

namespace LSDR.SDK.Entities
{
    public class TriggerTeleport : BaseTrigger
    {
        public Vector3 Destination;
        public bool AlwaysOn;

        protected override Color _editorColour { get; } = new Color(r: 0, g: 0.8f, b: 0.8f);

        protected override void onTrigger(Collider player)
        {
            if (!AlwaysOn && !DreamControlManager.Managed.TeleportsArmed) return;

            CharacterController controller = player.GetComponent<CharacterController>();
            controller.enabled = false;
            player.transform.position = Destination + Vector3.up * controller.skinWidth;
            controller.enabled = true;
        }

        protected override void onTriggerExit(Collider player) { }
    }
}