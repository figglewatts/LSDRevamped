using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LSDR.SDK.Data
{
    [CreateAssetMenu(menuName = "LSDR SDK/Graph spawn map")]
    public class GraphSpawnMap : ScriptableObject
    {
        /// <summary>
        ///     The size of the graph on both X and Y axes.
        /// </summary>
        public const int GRAPH_SIZE = 19;

        /// <summary>
        ///     The pool of dreams to choose from.
        /// </summary>
        public List<DreamElement> Dreams = new List<DreamElement>();

        /// <summary>
        ///     The layout of the graph. Elements are indices into Dreams list. -1 means random dream from pool.
        /// </summary>
        [SerializeField] private int[] _graph = new int[GRAPH_SIZE * GRAPH_SIZE];

        private float _createdOpacity;

        private Texture2D _createdTexture;
        private bool _dirty = true;

        // get a dream from the current graph position (i.e. -9 to +9)
        public Dream Get(int x, int y)
        {
            int graphX = x + 9;
            int graphY = y + 9;

            int graphIdx = _graph[graphY * GRAPH_SIZE + graphX];
            return graphIdx == -1 ? null : Dreams[graphIdx].Dream;
        }

        public void SetGraphSquare(int x, int y, int dreamIdx)
        {
            _graph[y * GRAPH_SIZE + x] = dreamIdx;
            _dirty = true;
        }

        public void ClearGraphSquare(int x, int y)
        {
            _graph[y * GRAPH_SIZE + x] = -1;
            _dirty = true;
        }

        public void ClearAllGraphSquares()
        {
            for (int y = 0; y < GRAPH_SIZE; y++)
            for (int x = 0; x < GRAPH_SIZE; x++)
                _graph[y * GRAPH_SIZE + x] = -1;
        }

        public void AddDreamsFromJournal(DreamJournal journal)
        {
            Dreams.Clear();
            foreach (Dream dream in journal.Dreams)
            {
                DreamElement dreamElement = new DreamElement(dream, new Color(Random.value, Random.value,
                    Random.value, a: 1));
                Dreams.Add(dreamElement);
            }

            _dirty = true;
        }

        public void AddDream()
        {
            DreamElement dreamElement = new DreamElement(new Color(Random.value, Random.value,
                Random.value, a: 1));
            Dreams.Add(dreamElement);
            _dirty = true;
        }

        public void AddDream(Dream dream, Color displayCol)
        {
            Dreams.Add(new DreamElement(dream, displayCol));
            _dirty = true;
        }

        public void RemoveDream(int dreamIdx)
        {
            Dreams.RemoveAt(dreamIdx);
            for (int y = 0; y < GRAPH_SIZE; y++)
            for (int x = 0; x < GRAPH_SIZE; x++)
                if (_graph[y * GRAPH_SIZE + x] == dreamIdx)
                    _graph[y * GRAPH_SIZE + x] = -1;
                else if (_graph[y * GRAPH_SIZE + x] > dreamIdx) _graph[y * GRAPH_SIZE + x]--;

            _dirty = true;
        }

        public void RemoveAllDreams()
        {
            ClearAllGraphSquares();
            Dreams.Clear();
        }

        public void ModifyColor(int dreamIdx, Color c)
        {
            Dreams[dreamIdx].Display = c;
            _dirty = true;
        }

        public Texture2D GetTexture(float opacity = 1f)
        {
            // if the data hasn't changed, and the opacity is the same, then return the previously created texture
            // as creating a new texture every time we want to draw will be expensive!
            if (_createdTexture && !_dirty && Math.Abs(opacity - _createdOpacity) < float.Epsilon)
                return _createdTexture;

            Texture2D tex = new Texture2D(GRAPH_SIZE, GRAPH_SIZE, TextureFormat.RGBA32, mipChain: false)
            {
                filterMode = FilterMode.Point
            };
            for (int y = 0; y < GRAPH_SIZE; y++)
            for (int x = 0; x < GRAPH_SIZE; x++)
                if (_graph[y * GRAPH_SIZE + x] == -1)
                    tex.SetPixel(x, y, Color.clear);
                else
                {
                    Color col = Dreams[_graph[y * GRAPH_SIZE + x]].Display;
                    col.a = opacity;
                    tex.SetPixel(x, y, col);
                }

            tex.Apply();

            _createdTexture = tex;
            _createdOpacity = opacity;
            _dirty = false;
            return tex;
        }

        [Serializable]
        public class DreamElement
        {
            public Dream Dream;
            public Color Display;

            public DreamElement(Color display) : this(dream: null, display) { }

            public DreamElement(Dream dream, Color display)
            {
                Dream = dream;
                Display = display;
            }
        }
    }
}
