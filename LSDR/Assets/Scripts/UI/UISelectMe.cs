using UnityEngine;
using UnityEngine.UI;

namespace LSDR.UI
{
	/// <summary>
	/// When attached to a UI object, makes it so that this object is initially selected.
	/// </summary>
	public class UISelectMe : MonoBehaviour
	{
		void Start() { GetComponent<Selectable>().Select(); }
		
		void OnEnable() { GetComponent<Selectable>().Select(); }
	}
}