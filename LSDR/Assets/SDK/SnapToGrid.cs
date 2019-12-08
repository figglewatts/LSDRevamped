using UnityEngine;

namespace LSDR.SDK
{
    [ExecuteInEditMode]
    public class SnapToGrid : MonoBehaviour
    {
        public static bool Enabled = false;
        public static Vector3 Offset = new Vector3(0.5f, 0, 0.5f);
        public static float Resolution = 1f;

#if UNITY_EDITOR
        public void Update()
        {
            if (Enabled)
            {
                transform.position = roundTransform(transform.position - Offset, 1f * Resolution) + Offset;
            }
        }

        private Vector3 roundTransform(Vector3 v, float snapValue)
        {
            return new Vector3
            {
                x = snapValue * Mathf.Round(v.x / snapValue),
                y = snapValue * Mathf.Round(v.y / snapValue),
                z = snapValue * Mathf.Round(v.z / snapValue)
            };
        }
#endif
    }
}
