using UnityEngine;
using LSDR.Game;
using LSDR.UI;

namespace LSDR.Entities.Dream
{
	// TODO: refactor TitleScreenPayloadReceiver in DreamDirector refactor
	public class TitleScreenPayloadReceiver : MonoBehaviour
	{
		public UIMainMenu MainMenu;
	
		private DreamPayload _receivedPayload;

		// Use this for initialization
		void Start()
		{
			_receivedPayload = GameObject.FindGameObjectWithTag("DreamPayload").GetComponent<DreamPayload>();
			if (_receivedPayload == null) return;

			if (!_receivedPayload.DreamEnded) return;

			MainMenu.ChangeMenuState(UIMainMenu.MenuState.GRAPH);

			DreamDirector.AddPayloadToPriorDreams(_receivedPayload);

			DreamDirector.CurrentDay++;

			SaveGameManager.SaveCurrentGame();

			// TODO: save game data to file

			// TODO: load from save file and populate graph

			_receivedPayload.ClearPayload();

			Fader.FadeOut(0.5F);
		}
	}
}