using System.Collections.Generic;
using UnityEngine;

namespace Torii.Event
{
    [CreateAssetMenu(menuName="Torii/Event")]
    public class ToriiEvent : ScriptableObject
    {
        /// <summary>
        /// The list of listeners that this event will notify if it is raised.
        /// </summary>
        private readonly List<ToriiEventListener> eventListeners = new List<ToriiEventListener>();

        public void Raise()
        {
            for(int i = eventListeners.Count -1; i >= 0; i--)
                eventListeners[i].OnEventRaised();
        }

        public void RegisterListener(ToriiEventListener listener)
        {
            if (!eventListeners.Contains(listener))
                eventListeners.Add(listener);
        }

        public void UnregisterListener(ToriiEventListener listener)
        {
            if (eventListeners.Contains(listener))
                eventListeners.Remove(listener);
        }
    }
}