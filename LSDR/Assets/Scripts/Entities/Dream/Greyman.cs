using LSDR.Dream;
using Torii.UI;
using UnityEngine;

namespace LSDR.Entities.Dream
{
    [RequireComponent(typeof(MeshRenderer))]
    public class Greyman : MonoBehaviour
    {
        public const int UPPERNESS_PENALTY = -9;
        public float MoveSpeed = 0.5f;
        public float FlashDistance = 4;
        public DreamSystem DreamSystem;
        public MeshRenderer Renderer;

        protected bool _playerEncountered;

        public void Update()
        {
            if (_playerEncountered) return;

            Transform t = transform;
            t.position += t.forward * (MoveSpeed * Time.deltaTime);

            float distanceToPlayer = Vector3.Distance(t.position, DreamSystem.Player.transform.position);

            if (distanceToPlayer < FlashDistance)
            {
                _playerEncountered = true;
                playerEncountered();
            }
        }

        private void playerEncountered()
        {
            DreamSystem.CurrentSequence.LogGraphContributionFromEntity(dynamicness: 0, UPPERNESS_PENALTY);
            ToriiFader.Instance.FadeIn(Color.white, duration: 0.1F, () =>
            {
                Renderer.enabled = false;
                ToriiFader.Instance.FadeOut(Color.white, duration: 3F, () =>
                {
                    if (gameObject != null) Destroy(gameObject);
                });
            });
        }
    }
}
