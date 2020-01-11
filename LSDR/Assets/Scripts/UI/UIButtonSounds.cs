using System;
using Torii.Audio;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LSDR.UI
{
    [RequireComponent(typeof(Button))]
    public class UIButtonSounds : MonoBehaviour
    {
        public AudioClip OnHover;
        public AudioClip OnPress;

        private Button _button;
        private EventTrigger _trigger;

        public void Start()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(() =>
            {
                if (OnPress != null)
                {
                    AudioPlayer.Instance.PlayClip(OnPress, mixerGroup: "SFX");
                }
            });
            hookUpEventTrigger();
        }

        private void hookUpEventTrigger()
        {
            if (_trigger != null) return;

            _trigger = gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry {eventID = EventTriggerType.PointerEnter};
            entry.callback.AddListener((data) =>
            {
                if (OnHover != null)
                {
                    AudioPlayer.Instance.PlayClip(OnHover, mixerGroup: "SFX");
                }
            });
            _trigger.triggers.Add(entry);
        }
    }
}
