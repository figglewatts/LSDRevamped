using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace UI
{
	public class UICardView : MonoBehaviour
	{
		public Button nextButton;
		public Button prevButton;

		public List<GameObject> Cards = new List<GameObject>();

		private int currentCard = 0;

		// Use this for initialization
		void Start()
		{
			prevButton.interactable = false;
			nextButton.interactable = true;
		}

		public void NextCard()
		{
			currentCard++;
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

		public void PrevCard()
		{
			currentCard--;
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
