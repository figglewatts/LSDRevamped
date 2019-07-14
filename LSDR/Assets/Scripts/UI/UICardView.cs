using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace LSDR.UI
{
	/// <summary>
	/// Defines a UI view with 'cards' that are swapped out by a next and a previous button.
	/// </summary>
	public class UICardView : MonoBehaviour
	{
		public Button nextButton;
		public Button prevButton;

		/// <summary>
		/// The list of 'cards' (as GameObjects) to use. Set in inspector.
		/// </summary>
		public List<GameObject> Cards = new List<GameObject>();

		private int currentCard = 0;

		// Use this for initialization
		void Start()
		{
			prevButton.interactable = false;
			nextButton.interactable = true;
		}

		/// <summary>
		/// Switch to the next card in the view.
		/// </summary>
		public void NextCard()
		{
			currentCard++;
			
			// set button state based on which card we're on
			if (currentCard == Cards.Count - 1)
			{
				nextButton.interactable = false;
			}
			if (currentCard > 0)
			{
				prevButton.interactable = true;
			}
			
			Cards[currentCard].SetActive(true);
			Cards[currentCard - 1].SetActive(false);
		}

		/// <summary>
		/// Switch to the previous card in the view.
		/// </summary>
		public void PrevCard()
		{
			currentCard--;
			
			// set button state based on which card we're on
			if (currentCard < Cards.Count)
			{
				nextButton.interactable = true;
			}
			if (currentCard <= 0)
			{
				prevButton.interactable = false;
			}
			
			Cards[currentCard].SetActive(true);
			Cards[currentCard + 1].SetActive(false);
		}
	}
}
