using UnityEngine;
using System.Collections;
using LSDR.Game;
using LSDR.Util;

namespace LSDR.Entities.Dream
{
	// TODO: refactor GreymanController to be more reliable
	public class GreymanController : MonoBehaviour
	{
		public float DistanceFromPlayer = 10F;

		[SerializeField] private float _minWaitTime;
		[SerializeField] private float _maxWaitTime;

		// Use this for initialization
		void Start() { StartCoroutine(RollForGreyman()); }

		public void SpawnGreyman()
		{
			Vector3 pos = DreamDirector.Player.transform.position +
			              (DreamDirector.Player.transform.forward * DistanceFromPlayer);
			Vector3 forward = DreamDirector.Player.transform.position - pos;
			Quaternion rot = Quaternion.LookRotation(forward, Vector3.up);
			EntityInstantiator.InstantiatePrefab("Prefabs/Greyman", pos, rot);
		}

		private IEnumerator RollForGreyman()
		{
			// while (true)
			// {
			// 	if (DreamDirector.CanSpawnGreyman)
			// 	{
			// 		int chance = RandUtil.Int(GameSettings.CHANCE_FOR_GREYMAN);
			//
			// 		Debug.Log(chance);
			// 		if (chance == 0) SpawnGreyman();
			// 	}
			//
			// 	yield return new WaitForSeconds(RandUtil.Float(_minWaitTime, _maxWaitTime));
			// }
			yield return null;
		}
	}
}