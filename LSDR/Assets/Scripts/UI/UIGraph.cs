using System.Collections.Generic;
using LSDR.Entities.Dream;
using UnityEngine;

namespace LSDR.UI
{
	/// <summary>
	/// The dream graph.
	/// TODO: refactor UIGraph with DreamDirector refactor
	/// </summary>
	public class UIGraph : MonoBehaviour
	{
		public UIMainMenu MainMenu;
		public Transform GraphSquareContainer;

		private GameObject _graphSquarePrefab;
		private List<Vector2> _squaresAlreadyInstantiated;
		private List<GameObject> _instantiatedObjects;

		public void Awake()
		{
			_graphSquarePrefab = Resources.Load<GameObject>("Prefabs/UI/GraphSquare");
			_squaresAlreadyInstantiated = new List<Vector2>();
			_instantiatedObjects = new List<GameObject>();
		}

		public void OnEnable() { InstantiateGraphSquares(); }

		public void InstantiateGraphSquares()
		{
			foreach (GameObject go in _instantiatedObjects) Destroy(go);
			_instantiatedObjects.Clear();
		
			_squaresAlreadyInstantiated.Clear();
			for (int i = 0; i < DreamDirector.GraphSquares.Count; i++)
			{
				Vector2 square = DreamDirector.GraphSquares[i];
				InstantiateGraphSquare(square, i == DreamDirector.GraphSquares.Count - 1);
				_squaresAlreadyInstantiated.Add(square);
			}
		}

		public void GoBackButtonPressed()
		{
			Fader.FadeIn(Color.black, 0.5F, () =>
			{
				MainMenu.ChangeMenuState(UIMainMenu.MenuState.MAIN);
				Fader.FadeOut(0.5F);
			});
		}

		private void InstantiateGraphSquare(Vector2 pos, bool mostRecent)
		{
			GameObject square = (GameObject)Instantiate(_graphSquarePrefab, GraphSquareContainer, false);
			UIGraphSquare squareScript = square.GetComponent<UIGraphSquare>();
			squareScript.Position = pos;
			squareScript.MostRecent = mostRecent;

			foreach (Vector2 gs in _squaresAlreadyInstantiated)
			{
				if (pos == gs) squareScript.ColourModifier--;
			}

			_instantiatedObjects.Add(square);
		}
	}
}
