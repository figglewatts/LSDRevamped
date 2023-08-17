using UnityEngine;
using UnityEngine.Events;

namespace Torii.Event
{
    [AddComponentMenu("Torii/EventListener")]
    public class ToriiEventListener : MonoBehaviour
    {
        public ToriiEvent Event;

        public UnityEvent Response;

        private void OnEnable()
        {
            Event.RegisterListener(this);
        }

        private void OnDisable()
        {
            Event.UnregisterListener(this);
        }

        public void OnEventRaised()
        {
            Response.Invoke();
        }
    }
}
