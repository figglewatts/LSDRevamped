using LSDR.SDK.Data;
using LSDR.SDK.DreamControl;
using UnityEngine;

namespace LSDR.SDK.Entities
{
    public class GraphContributor : MonoBehaviour
    {
        public GraphContribution Contribution;
        public float ContributionRadius = 5;
        public BaseEntity SourceEntity;

        protected const float UPDATE_INTERVAL = 0.25f;

        protected Transform _player;
        protected IDreamController _dreamController;
        protected float _t;
        protected bool _contributed = false;

        public void Start()
        {
            _player = EntityIndex.Instance.Get("__player").transform;
            _dreamController = DreamControlManager.Managed;
        }

        public void Update()
        {
            if (!_dreamController.InDream) return;
            if (_contributed) return;

            _t += Time.deltaTime;
            if (_player != null && _t > UPDATE_INTERVAL)
            {
                _t = 0;
                processPlayerPosition();
            }
        }

        public void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(r: 1, g: 0, b: 0, a: 0.6f);
            Gizmos.DrawWireSphere(transform.position, ContributionRadius);
        }

        protected void processPlayerPosition()
        {
            // get the player's position and calculate which LBD tile we're standing on
            Vector3 playerPos = _player.position;
            var distance = (transform.position - playerPos).sqrMagnitude;

            if (distance < (ContributionRadius * ContributionRadius))
            {
                _contributed = true;
                _dreamController.LogGraphContributionFromEntity(Contribution.Dynamic, Contribution.Upper,
                    SourceEntity);
            }
        }
    }
}
