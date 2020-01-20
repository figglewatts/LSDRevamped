using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace LSDR.UI
{
	/// <summary>
	/// When attached to a UI object, makes it so that this object is initially selected.
	/// </summary>
	public class UISelectMe : MonoBehaviour
	{
		void Start() { StartCoroutine(waitFrameThenSelect()); }

		void OnEnable() { StartCoroutine(waitFrameThenSelect()); }

		private IEnumerator waitFrameThenSelect()
		{
			yield return null;
			GetComponent<Selectable>().Select();
		}
	}
}