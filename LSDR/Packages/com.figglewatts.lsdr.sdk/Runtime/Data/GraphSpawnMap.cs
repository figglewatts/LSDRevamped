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
        /// The pool of dreams to choose from.
        /// </summary>
        public List<DreamElement> Dreams;

        /// <summary>
        /// The size of the graph on both X and Y axes.
        /// </summary>
        public const int GRAPH_SIZE = 19;

        /// <summary>
        /// The layout of the graph. Elements are indices into Dreams list. -1 means random dream from pool.
        /// </summary>
        [SerializeField] private int[] _graph;

        private Texture2D _createdTexture;
        private float _createdOpacity;
        private bool _dirty = true;

        public GraphSpawnMap()
        {
            Dreams = new List<DreamElement>();
            _graph = new int[GRAPH_SIZE * GRAPH_SIZE];
            ClearAllGraphSquares();
        }

        public Dream Get(int x, int y)
        {
            int graphIdx = _graph[y * GRAPH_SIZE + x];
            return graphIdx == -1 ? null : Dreams[graphIdx].Dream;
            ;
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
            {
                for (int x = 0; x < GRAPH_SIZE; x++)
                {
                    _graph[y * GRAPH_SIZE + x] = -1;
                }
            }
        }

        public void AddDreamsFromJournal(DreamJournal journal)
        {
            Dreams.Clear();
            foreach (var dream in journal.Dreams)
            {
                var dreamElement = new DreamElement(dream, new Color(Random.value, Random.value,
                    Random.value, 1));
                Dreams.Add(dreamElement);
            }

            _dirty = true;
        }

        public void AddDream()
        {
            var dreamElement = new DreamElement(new Color(Random.value, Random.value,
                Random.value, 1));
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
            {
                for (int x = 0; x < GRAPH_SIZE; x++)
                {
                    if (_graph[y * GRAPH_SIZE + x] == dreamIdx)
                    {
                        _graph[y * GRAPH_SIZE + x] = -1;
                    }
                    else if (_graph[y * GRAPH_SIZE + x] > dreamIdx)
                    {
                        _graph[y * GRAPH_SIZE + x]--;
                    }
                }
            }

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
            if (!_dirty && Math.Abs(opacity - _createdOpacity) < float.Epsilon) return _createdTexture;

            Texture2D tex = new Texture2D(GRAPH_SIZE, GRAPH_SIZE, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Point
            };
            for (int y = 0; y < GRAPH_SIZE; y++)
            {
                for (int x = 0; x < GRAPH_SIZE; x++)
                {
                    if (_graph[y * GRAPH_SIZE + x] == -1)
                    {
                        tex.SetPixel(x, y, Color.clear);
                    }
                    else
                    {
                        Color col = Dreams[_graph[y * GRAPH_SIZE + x]].Display;
                        col.a = opacity;
                        tex.SetPixel(x, y, col);
                    }
                }
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

            public DreamElement(Color display) : this(null, display) { }

            public DreamElement(Dream dream, Color display)
            {
                Dream = dream;
                Display = display;
            }
        }
    }
}
