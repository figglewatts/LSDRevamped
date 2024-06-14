using LSDR.Dream;
using Torii.UI;
using UnityEngine;

namespace LSDR.Entities.Dream
{
    public class Greyman : MonoBehaviour
    {
        public float MoveSpeed = 0.3f;
        public float FlashDistance = 2;
        public DreamSystem DreamSystem;
        public GameObject GreymanObject;

        protected bool _playerEncountered;

        public void Update()
        {
            if (_playerEncountered || !DreamSystem.InDream || DreamSystem.Player == null) return;

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
            DreamSystem.LogGraphContributionFromArea(-10, -10);
            DreamSystem.DisableFlashbackForDays(10);
            Debug.Log("encountered player");
            ToriiFader.Instance.FadeIn(Color.white, duration: 0.1F, () =>
            {
                Debug.Log("Setting grey man to inactive");
                GreymanObject.SetActive(false);
                ToriiFader.Instance.FadeOut(Color.white, duration: 3F, () =>
                {
                    Debug.Log("Finished fading out, destroying if not null");
                    if (gameObject != null)
                    {
                        Debug.Log("destroying");
                        Destroy(gameObject);
                    }
                });
            }, forced: false);
        }
    }
}
