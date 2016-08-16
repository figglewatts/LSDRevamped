﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace UI
{
	public class UIGraphSquare : MonoBehaviour
	{
		[Range(1, 5)] public int ColourModifier;
		public Vector2 Position;
		public bool MostRecent;

		private Image _graphSquareImage;

		private const float SQUARE_DIMENSION = 16;
		private const float FLASH_TIME = 0.25F;
		private const float ANCHOR_OFFSET = 9;

		public void Awake()
		{
			ColourModifier = 5;
			_graphSquareImage = GetComponent<Image>();
		}

		public void Start()
		{
			_graphSquareImage.rectTransform.anchoredPosition = new Vector2((Position.x + ANCHOR_OFFSET)*SQUARE_DIMENSION,
				(Position.y - ANCHOR_OFFSET)*SQUARE_DIMENSION);
			_graphSquareImage.rectTransform.localScale = Vector3.one;
			_graphSquareImage.rectTransform.sizeDelta = new Vector2(SQUARE_DIMENSION, SQUARE_DIMENSION);
		}

		public void Update()
		{
			if (MostRecent)
			{
				if (Time.timeSinceLevelLoad%FLASH_TIME > FLASH_TIME/2) { _graphSquareImage.color = Color.red; Debug.Log("RED");} 
				else _graphSquareImage.color = new Color(0.2F*ColourModifier, 0.2F*ColourModifier, 0.2F*ColourModifier);
			}
			else _graphSquareImage.color = new Color(0.2F * ColourModifier, 0.2F * ColourModifier, 0.2F * ColourModifier);
		}
	}
}