using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LSDR.Dream;
using LSDR.Game;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Video;

namespace LSDR.UI.Graph
{
    /// <summary>
    ///     The dream graph.
    /// </summary>
    public class UIGraph : MonoBehaviour
    {
        // squares get a shade darker every this many days
        protected const int DAYS_PER_SHADE = 10;

        public Transform GraphSquareContainer;
        public GameSaveSystem GameSave;
        public SettingsSystem Settings;
        public DreamSystem DreamSystem;

        // the points that make up the face, if they've all been visited
        protected static readonly Vector2[] _facePoints =
        {
            new Vector2(-1, 1), new Vector2(1, 1), new Vector2(0, 0), new Vector2(0, -3)
        };

        private GameObject _graphSquarePrefab;
        private List<GameObject> _instantiatedObjects;
        private List<Vector2> _squaresAlreadyInstantiated;

        public void Awake()
        {
            _graphSquarePrefab = Resources.Load<GameObject>("Prefabs/UI/GraphSquare");
            _squaresAlreadyInstantiated = new List<Vector2>();
            _instantiatedObjects = new List<GameObject>();
        }

        public void OnEnable()
        {
            InstantiateGraphSquares();

            List<VideoClip> faceVideos = Settings.CurrentJournal.ChartFaceVideos;
            bool faceVisited = _facePoints.All(p => _squaresAlreadyInstantiated.Contains(p));
            if (!GameSave.CurrentJournalSave.SeenChartFace && faceVideos != null && faceVideos.Count > 0 &&
                faceVisited)
            {
                StartCoroutine(showFace(faceVideos));
            }
        }

        protected IEnumerator showFace(List<VideoClip> faceVideos)
        {
            // stop the graph being closed while the face lights up
            EventSystem eventSystem = EventSystem.current;
            eventSystem.enabled = false;

            yield return new WaitForSeconds(1.5f);
            foreach (Vector2 point in _facePoints.OrderBy(p => _squaresAlreadyInstantiated.IndexOf(p)))
            {
                foreach (GameObject square in _instantiatedObjects)
                {
                    UIGraphSquare squareScript = square.GetComponent<UIGraphSquare>();
                    if (squareScript.Position == point) squareScript.Highlighted = true;
                }
                yield return new WaitForSeconds(1.2f);
            }

            if (eventSystem != null) eventSystem.enabled = true;

            // the game may have been torn down part way through, i.e. when leaving play mode
            if (GameSave.CurrentJournalSave == null) yield break;

            GameSave.CurrentJournalSave.SeenChartFace = true;
            GameSave.Save();
            DreamSystem.PlayVideos(faceVideos);
        }

        public void InstantiateGraphSquares()
        {
            foreach (GameObject go in _instantiatedObjects) Destroy(go);
            _instantiatedObjects.Clear();

            _squaresAlreadyInstantiated.Clear();
            Vector2[] coords = getGraphCoords();
            for (int i = 0; i < coords.Length; i++)
            {
                int daysAgo = coords.Length - 1 - i;
                InstantiateGraphSquare(coords[i], daysAgo);
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

        private void InstantiateGraphSquare(Vector2 pos, int daysAgo)
        {
            GameObject square = Instantiate(_graphSquarePrefab, GraphSquareContainer, worldPositionStays: false);
            UIGraphSquare squareScript = square.GetComponent<UIGraphSquare>();
            squareScript.Position = pos;
            squareScript.MostRecent = daysAgo == 0;

            // darker the longer ago it was, newer squares are drawn over older ones in the same place
            squareScript.ColourModifier = Mathf.Max(1, 5 - daysAgo / DAYS_PER_SHADE);

            _instantiatedObjects.Add(square);
        }
    }
}
