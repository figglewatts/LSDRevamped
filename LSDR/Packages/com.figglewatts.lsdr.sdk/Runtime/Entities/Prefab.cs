using UnityEngine;

namespace LSDR.SDK.Entities
{
    public class Prefab : BaseEntity
    {
        public GameObject Object;

        public GameObject CreateInstance()
        {
            var obj = Instantiate(Object);
            obj.SetActive(true);
            return obj;
        }
    }
}
