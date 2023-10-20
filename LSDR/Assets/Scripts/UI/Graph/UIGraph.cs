using System.Collections.Generic;
using LSDR.Dream;
using LSDR.Game;
using UnityEngine;

namespace LSDR.UI.Graph
{
    /// <summary>
    ///     The dream graph.
    /// </summary>
    public class UIGraph : MonoBehaviour
    {
        public Transform GraphSquareContainer;
        public GameSaveSystem GameSave;

        private GameObject _graphSquarePrefab;
        private List<GameObject> _instantiatedObjects;
        private List<Vector2> _squaresAlreadyInstantiated;

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
            Vector2[] coords = getGraphCoords();
            for (int i = 0; i < coords.Length; i++)
            {
                bool mostRecent = i == coords.Length - 1;
                InstantiateGraphSquare(coords[i], mostRecent);
                _squaresAlreadyInstantiated.Add(coords[i]);
            }
        }

        private Vector2[] getGraphCoords()
        {
            var coords = new Vector2[GameSave.CurrentJournalSave.NumberOfSequences];
            for (int i = 0; i < GameSave.CurrentJournalSave.NumberOfSequences; i++)
            {
                DreamSequence seq = GameSave.CurrentJournalSave.GetSequence(i);
                coords[i] = seq.EvaluateGraphPosition();
            }

            return coords;
        }

        private void InstantiateGraphSquare(Vector2 pos, bool mostRecent)
        {
            GameObject square = Instantiate(_graphSquarePrefab, GraphSquareContainer, worldPositionStays: false);
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
