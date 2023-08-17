using System;
using UnityEngine;

namespace LSDR.SDK.Data
{
    /// <summary>
    ///     Used to store something's contribution to the dream graph values. These are collected in a list in the dream
    ///     and totalled up whenever something needs the current graph position.
    /// </summary>
    [Serializable]
    public struct GraphContribution
    {
        [SerializeField] private Vector2Int _vector;

        public GraphContribution(int dynamicness, int upperness)
        {
            _vector = new Vector2Int(dynamicness, upperness);
        }

        public int Dynamic => _vector.x;
        public int Upper => _vector.y;

        public static implicit operator Vector2Int(GraphContribution gc)
        {
            return gc._vector;
        }
    }
}
