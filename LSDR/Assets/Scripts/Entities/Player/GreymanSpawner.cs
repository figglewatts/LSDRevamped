using System.Collections.Generic;
using LSDR.Dream;
using LSDR.SDK.Util;
using Torii.Console;
using UnityEngine;

namespace LSDR.Entities.Dream
{
    public class GreymanSpawner : MonoBehaviour
    {
        private const float CHANCE_FOR_GREYMAN = 12;
        private const float DISTANCE_BETWEEN_SPAWN_CHANCES = 20;
        private const float MAX_DISTANCE_PER_FRAME = 5;
        public DreamSystem DreamSystem;
        public GameObject GreymanPrefab;
        public float GreymanSpawnDistance = 10;
        private List<GameObject> _greyMen = new List<GameObject>();
        private bool _spawnedThisVisit;
        private float _distanceWalked;
        private Vector3 _lastPosition;

        public void Start() { DevConsole.Register(this); }

        public void OnDestroy() { DevConsole.Deregister(this); }

        public void Update()
        {
            if (!DreamSystem.InDream || _spawnedThisVisit) return;

            // roll each time the player has walked a fair way, roughly every chunk in the original dreams
            Vector3 moved = transform.position - _lastPosition;
            moved.y = 0;
            _lastPosition = transform.position;

            // don't count teleports as walking
            if (moved.magnitude < MAX_DISTANCE_PER_FRAME) _distanceWalked += moved.magnitude;
            if (_distanceWalked < DISTANCE_BETWEEN_SPAWN_CHANCES) return;

            _distanceWalked = 0;
            RollSpawn();
        }

        [ContextMenu("Spawn")]
        [Console]
        public GameObject Spawn()
        {
            Debug.Log("Spawning grey man");
            Vector3 forward = transform.forward;
            Vector3 spawnPos = transform.position + forward * GreymanSpawnDistance + Vector3.up * 0.1f;
            Vector3 toPlayer = transform.position - spawnPos;
            toPlayer.y = 0;
            Quaternion orientation = Quaternion.LookRotation(toPlayer);
            var instance = Instantiate(GreymanPrefab, spawnPos, orientation);
            _greyMen.Add(instance);
            return instance;
        }

        public void RemoveGreyMen()
        {
            foreach (var greyMan in _greyMen)
            {
                Destroy(greyMan);
            }
            _greyMen.Clear();
        }

        public void OnNewDream()
        {
            // remove existing grey men
            RemoveGreyMen();
            _spawnedThisVisit = false;
            _distanceWalked = 0;
            _lastPosition = transform.position;

            // see if we can scare the player by loading into a grey man spawn ;)
            RollSpawn();
        }

        public void RollSpawn()
        {
            // he turns up at most once each time you enter a dream
            if (!DreamSystem.InDream || _spawnedThisVisit) return;
            if (DreamSystem.CurrentDay % 2 != 0) return; // only spawn on even days
            if (!DreamSystem.CurrentDream.GreyMan || !RandUtil.OneIn(CHANCE_FOR_GREYMAN)) return;

            Spawn();
            _spawnedThisVisit = true;
        }
    }
}