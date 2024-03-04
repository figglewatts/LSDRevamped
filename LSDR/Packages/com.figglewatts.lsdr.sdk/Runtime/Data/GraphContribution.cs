using System;
using LSDR.SDK.Entities;
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
        private string _entityID;
        private string _dream;

        public GraphContribution(int dynamicness, int upperness)
        {
            _vector = new Vector2Int(dynamicness, upperness);
            _playerPosition = Vector3.zero;
            _playerYRotation = 0;
            _entityID = null;
            _dream = null;
        }

        public GraphContribution(int dynamicness, int upperness, Transform playerTransform, BaseEntity sourceEntity,
            string dream)
            : this(dynamicness, upperness)
        {
            _playerPosition = playerTransform.position;
            _playerYRotation = playerTransform.eulerAngles.y;
            _entityID = sourceEntity.ID;
            _dream = dream;
        }

        public GraphContribution(int dynamicness, int upperness, Vector3 playerPosition, float playerYRotation,
            string entityID, string dream)
            : this(dynamicness, upperness)
        {
            _playerPosition = playerPosition;
            _playerYRotation = playerYRotation;
            _entityID = entityID;
            _dream = dream;
        }

        public int Dynamic => _vector.x;
        public int Upper => _vector.y;

        public Vector3 PlayerPosition => _playerPosition;

        public float PlayerYRotation => _playerYRotation;

        public string EntityID => _entityID;

        public string Dream => _dream;
    }
}
