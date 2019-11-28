using UnityEngine;

namespace Torii.Event
{
    public class TriggerOnStart : MonoBehaviour
    {
        public ToriiEvent ToTrigger;

        public void Start()
        {
            ToTrigger.Raise();
        }
    }
}
