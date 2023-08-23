using UnityEngine;

namespace LSDR.SDK.Entities
{
    [RequireComponent(typeof(BoxCollider))]
    public abstract class BaseTrigger : BaseEntity
    {
        public bool OnlyOnce;

        protected BoxCollider _collider;
        protected bool _triggered;
        protected bool _exitTriggered;

        protected abstract Color _editorColour { get; }

        public override void Start()
        {
            base.Start();
            _collider = GetComponent<BoxCollider>();
            _collider.isTrigger = true;
        }

        public void OnDrawGizmos()
        {
            Vector3 position = transform.position;
            Gizmos.color = _editorColour;
            Vector3 localScale = transform.localScale;
            Gizmos.DrawWireCube(position, localScale);
            Gizmos.color = new Color(_editorColour.r, _editorColour.g, _editorColour.b, _editorColour.a * 0.25f);
            Gizmos.DrawCube(position, localScale);
        }

        public void OnTriggerEnter(Collider other)
        {
            // ignore if already triggered and in only once mode
            if (OnlyOnce && _triggered) return;

            // ignore if not player
            if (!other.gameObject.CompareTag("Player")) return;

            onTrigger(other);
            _triggered = true;
        }

        public void OnTriggerExit(Collider other)
        {
            // ignore if already triggered and in only once mode
            if (OnlyOnce && _exitTriggered) return;

            // ignore if not player
            if (!other.gameObject.CompareTag("Player")) return;

            onTriggerExit(other);
            _exitTriggered = true;
        }

        protected abstract void onTrigger(Collider player);

        protected abstract void onTriggerExit(Collider player);
    }
}
