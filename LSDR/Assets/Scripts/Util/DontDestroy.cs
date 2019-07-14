using UnityEngine;

namespace LSDR.Util
{
	/// <summary>
	/// If attached to a GameObject, won't destroy it on scene load. Also ensures that there isn't an existing
	/// instance of the GameObject already in the newly loaded scene.
	/// </summary>
	public class DontDestroy : MonoBehaviour
	{
		void Awake()
		{
			// make sure there isn't another one
			GameObject obj = GameObject.Find(this.gameObject.name);
			if (!obj){ return; }

			int myId = this.gameObject.GetInstanceID();
			int otherId = obj.GetInstanceID();

			if (myId == otherId){ return; }
			
			if (myId > otherId)
			{
				DestroyImmediate(this.gameObject);
			}
		}

		void Start()
		{
			DontDestroyOnLoad(this.gameObject);
		}
	}
}