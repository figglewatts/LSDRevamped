using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Torii.UI
{
    public class ContainerPopulator : MonoBehaviour, IPopulator<GameObject>
    {
        public void Populate(List<GameObject> with)
        {
            Clear();
            foreach (GameObject obj in with)
            {
                obj.SetActive(true);
            }
        }

        public void Clear()
        {
            foreach (Transform obj in transform)
            {
                if (obj.gameObject.activeSelf) Destroy(obj.gameObject);
            }
        }
    }
}