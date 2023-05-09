using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Torii.UI
{
    /// <summary>
    /// Used to populate a UI container with GameObjects.
    /// </summary>
    public class ContainerPopulator : MonoBehaviour, IPopulator<GameObject>
    {
        /// <summary>
        /// Put these GameObjects in the container.
        /// </summary>
        /// <param name="with">The GameObjects to put in the container.</param>
        public void Populate(List<GameObject> with)
        {
            Clear();
            foreach (GameObject obj in with)
            {
                obj.transform.SetParent(transform);
                obj.SetActive(true);
            }
        }

        /// <summary>
        /// Remove the GameObjects from the container.
        /// </summary>
        public void Clear()
        {
            foreach (Transform obj in transform)
            {
                if (obj.gameObject.activeSelf) Destroy(obj.gameObject);
            }
        }
    }
}