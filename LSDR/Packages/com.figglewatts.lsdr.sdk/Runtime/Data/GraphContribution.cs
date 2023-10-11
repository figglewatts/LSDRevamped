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
        private Vector3 _playerPosition;
        private float _playerYRotation;

        public GraphContribution(int dynamicness, int upperness)
        {
            _vector = new Vector2Int(dynamicness, upperness);
            _playerPosition = Vector3.zero;
            _playerYRotation = 0;
        }

        public GraphContribution(int dynamicness, int upperness, Transform playerTransform)
            : this(dynamicness, upperness)
        {
            _playerPosition = playerTransform.position;
            _playerYRotation = playerTransform.eulerAngles.y;
        }

        public GraphContribution(int dynamicness, int upperness, Vector3 playerPosition, float playerYRotation)
            : this(dynamicness, upperness)
        {
            _playerPosition = playerPosition;
            _playerYRotation = playerYRotation;
        }

        public int Dynamic => _vector.x;
        public int Upper => _vector.y;

        public Vector3 PlayerPosition => _playerPosition;

        public float PlayerYRotation => _playerYRotation;
    }
}
