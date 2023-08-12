using UnityEngine;

namespace Torii.Event
{
    [AddComponentMenu("Torii/TriggerOnKeypress")]
    public class TriggerOnKeypress : MonoBehaviour
    {
        public KeyCode Key;
        public ToriiEvent Event;

        public void Update()
        {
            if (Input.GetKeyDown(Key))
            {
                Event.Raise();
            }
        }
    }
}
