using UnityEngine;
using LSDR.Entities.Dream;
using LSDR.Game;
using LSDR.UI;

namespace LSDR.Entities.WorldObject
{
	// TODO: refactor GraymanScript
	public class GreymanScript : MonoBehaviour
	{
		public Color GreymanColor;
		public GameObject GreymanMesh;

		private Material _greymanMaterial;

		private bool _playerEncountered = false;

		[SerializeField] private float _moveSpeed;
		[SerializeField] private float _flashDistance;

		// Use this for initialization
		void Start()
		{
			_greymanMaterial =
				new Material(Shader.Find(GameSettings.CurrentSettings.UseClassicShaders
					? "LSD/PSX/Diffuse"
					: "LSD/Diffuse"));
			_greymanMaterial.SetColor("_Tint", GreymanColor);

			GreymanMesh.GetComponent<Renderer>().material = _greymanMaterial;
		}

		// Update is called once per frame
		void Update()
		{
			transform.position += transform.forward * _moveSpeed * Time.deltaTime;

			float distanceToPlayer = Vector3.Distance(transform.position, DreamDirector.Player.transform.position);

			if (distanceToPlayer < _flashDistance && !_playerEncountered) PlayerEncountered();
		}

		private void PlayerEncountered()
		{
			_playerEncountered = true;
			DreamDirector.HappinessAccumulator += DreamDirector.GREYMAN_HAPPINESS_PENALTY;
			Fader.FadeIn(Color.white, 0.1F, () =>
			{
				GetComponentInChildren<Renderer>().enabled = false;
				Fader.FadeOut(Color.white, 3F, () => { Destroy(gameObject); });
			});
		}
	}
}