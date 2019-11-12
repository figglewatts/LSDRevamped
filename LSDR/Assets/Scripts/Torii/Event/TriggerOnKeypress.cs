using System;
using UnityEditor;
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
        
        [MenuItem("GameObject/Torii/TriggerOnKeypress", false, 11)]
        public static void Create(MenuCommand menuCommand)
        {
            GameObject eventOnKeypressObject = new GameObject("ToriiEventOnKeypress");
            GameObjectUtility.SetParentAndAlign(eventOnKeypressObject, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(eventOnKeypressObject, "Create " + eventOnKeypressObject.name);
            Selection.activeObject = eventOnKeypressObject;
            eventOnKeypressObject.AddComponent<TriggerOnKeypress>();

        }
    }
}