using UnityEngine;

namespace Util
{
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