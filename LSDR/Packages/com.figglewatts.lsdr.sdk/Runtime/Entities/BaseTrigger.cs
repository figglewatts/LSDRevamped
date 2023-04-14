using UnityEngine;

namespace LSDR.SDK.Entities
{
    [RequireComponent(typeof(BoxCollider))]
    public abstract class BaseTrigger : BaseEntity
    {
        public bool OnlyOnce;

        protected BoxCollider _collider;
        protected bool _triggered;

        public override void Start()
        {
            base.Start();
            _collider = GetComponent<BoxCollider>();
            _collider.isTrigger = true;
        }

        public void OnDrawGizmos()
        {
            Vector3 position = transform.position;
            Gizmos.color = Color.green;
            Vector3 localScale = transform.localScale;
            Gizmos.DrawWireCube(position, localScale);
            Gizmos.color = new Color(0, 1, 0, 0.5f);
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

        protected abstract void onTrigger(Collider player);
    }
}
