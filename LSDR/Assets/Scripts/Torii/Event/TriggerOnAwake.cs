using UnityEngine;

namespace Torii.Event
{
    public class TriggerOnAwake : MonoBehaviour
    {
        public ToriiEvent ToTrigger;

        public void Awake()
        {
            ToTrigger.Raise();
        }
    }
}