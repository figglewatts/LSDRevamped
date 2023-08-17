using UnityEngine;

namespace Torii.Util
{
    /// <summary>
    ///     If attached to a GameObject, won't destroy it on scene load. Also ensures that there isn't an existing
    ///     instance of the GameObject already in the newly loaded scene.
    /// </summary>
    public class DontDestroy : MonoBehaviour
    {
        private void Awake()
        {
            // make sure there isn't another one
            GameObject obj = GameObject.Find(gameObject.name);
            if (!obj) { return; }

            int myId = gameObject.GetInstanceID();
            int otherId = obj.GetInstanceID();

            if (myId == otherId) { return; }

            if (myId > otherId)
            {
                DestroyImmediate(gameObject);
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
