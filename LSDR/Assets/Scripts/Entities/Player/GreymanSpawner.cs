using System.Collections;
using LSDR.Dream;
using LSDR.SDK.Util;
using Torii.Console;
using UnityEngine;

namespace LSDR.Entities.Dream
{
    public class GreymanSpawner : MonoBehaviour
    {
        private const float MIN_WAIT_BETWEEN_SPAWN_CHANCES_SECONDS = 5;
        private const float MAX_WAIT_BETWEEN_SPAWN_CHANCES_SECONDS = 15;
        private const float CHANCE_FOR_GREYMAN = 7;
        public DreamSystem DreamSystem;
        public GameObject GreymanPrefab;
        public float GreymanSpawnDistance = 10;
        private Coroutine _rollForGreymanCoroutine;

        public void Start()
        {
            DevConsole.Register(this);

            if (!DreamSystem.InDream) return;

            if (_rollForGreymanCoroutine != null) StopCoroutine(_rollForGreymanCoroutine);

            _rollForGreymanCoroutine = StartCoroutine(RollForGreyman());
        }

        public void OnDestroy() { DevConsole.Deregister(this); }

        [ContextMenu("Spawn")]
        [Console]
        public void Spawn()
        {
            Debug.Log("Spawning grey man");
            Vector3 forward = transform.forward;
            Vector3 spawnPos = transform.position + forward * GreymanSpawnDistance;
            Vector3 toPlayer = transform.position - spawnPos;
            Quaternion orientation = Quaternion.LookRotation(toPlayer);
            Instantiate(GreymanPrefab, spawnPos, orientation);
        }

        public void OnNewDream()
        {
            // see if we can scare the player by loading into a grey man spawn ;)
            RollSpawn();
        }

        public IEnumerator RollForGreyman()
        {
            while (true)
            {
                RollSpawn();

                yield return new WaitForSeconds(RandUtil.Float(MIN_WAIT_BETWEEN_SPAWN_CHANCES_SECONDS,
                    MAX_WAIT_BETWEEN_SPAWN_CHANCES_SECONDS));
            }
            // ReSharper disable once IteratorNeverReturns
        }

        public void RollSpawn()
        {
            if (!DreamSystem.InDream) return;
            bool shouldSpawnGreyMan = RandUtil.OneIn(CHANCE_FOR_GREYMAN);
            if (DreamSystem.CurrentDream.GreyMan && shouldSpawnGreyMan)
            {
                Spawn();
            }
        }
    }
}
