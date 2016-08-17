using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace UI
{
	public class UISelectMe : MonoBehaviour
	{
		void Start() { GetComponent<Selectable>().Select(); }
		
		void OnEnable() { GetComponent<Selectable>().Select(); }
	}
}