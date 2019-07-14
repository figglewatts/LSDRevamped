using UnityEngine;

namespace Util
{
    /// <summary>
    /// MatchTransform matches the transform of the attached object to a given target.
    /// </summary>
    public class MatchTransform : MonoBehaviour
    {
        public bool MatchPosition
        {
            get { return MatchPosX && MatchPosY && MatchPosZ; }
            set
            {
                MatchPosX = value;
                MatchPosY = value;
                MatchPosZ = value;
            }
        }
        public bool MatchPosX;
        public bool MatchPosY;
        public bool MatchPosZ;

        public bool MatchRotation
        {
            get { return MatchRotX && MatchRotY && MatchRotZ; }
            set
            {
                MatchRotX = value;
                MatchRotY = value;
                MatchRotZ = value;
            }
        }
        public bool MatchRotX;
        public bool MatchRotY;
        public bool MatchRotZ;

        public bool MatchScale
        {
            get { return MatchScaleX && MatchScaleY && MatchScaleZ; }
            set
            {
                MatchScaleX = value;
                MatchScaleY = value;
                MatchScaleZ = value;
            }
        }
        public bool MatchScaleX;
        public bool MatchScaleY;
        public bool MatchScaleZ;

        public Transform Match;

        private Transform _this;

        private void Start() { _this = transform; }

        private void Update()
        {
            Vector3 pos = _this.position;
            if (MatchPosX)
            {
                pos.x = Match.position.x;
            }
            if (MatchPosY)
            {
                pos.y = Match.position.y;
            }
            if (MatchPosZ)
            {
                pos.z = Match.position.z;
            }
            _this.position = pos;

            Vector3 euler = _this.rotation.eulerAngles;
            if (MatchRotX)
            {
                euler.x = Match.rotation.eulerAngles.x;
            }
            if (MatchRotY)
            {
                euler.y = Match.rotation.eulerAngles.y;
            }
            if (MatchRotZ)
            {
                euler.z = Match.rotation.eulerAngles.z;
            }
            _this.rotation = Quaternion.Euler(euler);

            Vector3 scale = _this.localScale;
            if (MatchScaleX)
            {
                scale.x = Match.localScale.x;
            }
            if (MatchScaleY)
            {
                scale.y = Match.localScale.y;
            }
            if (MatchScaleZ)
            {
                scale.z = Match.localScale.z;
            }
            _this.localScale = scale;
        }
    }
}
