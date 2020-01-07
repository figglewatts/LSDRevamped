using System;
using System.Collections;
using LSDR.Dream;
using LSDR.Util;
using UnityEngine;

namespace LSDR.Entities.Dream
{
    public class GreymanSpawner : MonoBehaviour
    {
        public DreamSystem DreamSystem;
        public GameObject GreymanPrefab;
        public float GreymanSpawnDistance = 10;

        private const float MIN_WAIT_BETWEEN_SPAWN_CHANCES_SECONDS = 5;
        private const float MAX_WAIT_BETWEEN_SPAWN_CHANCES_SECONDS = 10;
        private const float CHANCE_FOR_GREYMAN = 100;

        [NonSerialized] private GameObject _spawnedGreyman;
        [NonSerialized] private bool _hasSpawnedGreyman;
        [NonSerialized] private Coroutine _rollForGreymanCoroutine;
        [NonSerialized] private Transform _playerTransform;
        
        public void OnLevelLoad()
        {
            // only start rolling for grey man if level has it enabled
            if (!DreamSystem.CurrentDream.GreyMan) return;
            
            if (_rollForGreymanCoroutine != null) StopCoroutine(_rollForGreymanCoroutine);

            //_rollForGreymanCoroutine = StartCoroutine(RollForGreyman());

            _playerTransform = transform;
            
            Spawn();
        }

        [ContextMenu("Spawn")]
        public void Spawn()
        {
            Debug.Log("Spawning grey man");
            
            if (_spawnedGreyman != null) Destroy(_spawnedGreyman);

            var forward = _playerTransform.forward;
            Vector3 spawnPos = _playerTransform.position + forward * GreymanSpawnDistance - new Vector3(0, 0.23f, 0);
            Quaternion spawnRot = Quaternion.LookRotation(forward * -1, Vector3.up);
            _spawnedGreyman = Instantiate(GreymanPrefab, spawnPos, spawnRot);
        }

        public IEnumerator RollForGreyman()
        {
            _hasSpawnedGreyman = false;
            while (true)
            {
                if (RandUtil.OneIn(CHANCE_FOR_GREYMAN) && !_hasSpawnedGreyman)
                {
                    Spawn();
                    _hasSpawnedGreyman = true;
                }
                yield return new WaitForSeconds(RandUtil.Float(MIN_WAIT_BETWEEN_SPAWN_CHANCES_SECONDS,
                    MAX_WAIT_BETWEEN_SPAWN_CHANCES_SECONDS));
            }
            // ReSharper disable once IteratorNeverReturns
        }
    }
}
