using UnityEngine;
using UnityEngine.UI;

namespace AutoUpdate
{
	public class UpdateMenu : MonoBehaviour
	{
		public GameObject confirmUpdateContainer;
		public GameObject updateInstallContainer;
		public GameObject successfulInstallContainer;

		public Slider downloadSlider;
		public Slider patchSlider;

		/// <summary>
		/// Called when the yes button is clicked on the confirm update menu.
		/// </summary>
		public void ConfirmUpdate()
		{
			StartCoroutine(GameObject.FindGameObjectWithTag("AutoUpdate").GetComponent<UpdaterScript>().DownloadPatches());
		}

		/// <summary>
		/// Called when the update is completed and the player clicks OK.
		/// </summary>
		public void UpdateCompleted()
		{
			Application.Quit();
		}
	}
}