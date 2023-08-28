using LSDR.SDK.Util;
using UnityEngine;

namespace LSDR.SDK.Entities
{
    public class ChanceToAppear : MonoBehaviour
    {
        public int OneIn;

        public void OnValidate()
        {
            if (OneIn < 1) OneIn = 1;
        }

        public void Start()
        {
            if (!RandUtil.OneIn(OneIn)) gameObject.SetActive(false);
        }
    }
}
