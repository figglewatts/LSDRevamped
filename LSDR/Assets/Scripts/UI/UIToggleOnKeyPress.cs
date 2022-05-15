using System;
using UnityEngine;

namespace LSDR.UI
{
    public class UIToggleOnKeyPress : MonoBehaviour
    {
        public GameObject Target;
        public KeyCode Key;

        public void Update()
        {
            if (Input.GetKeyDown(Key)) Target.SetActive(!Target.activeSelf);
        }
    }
}
