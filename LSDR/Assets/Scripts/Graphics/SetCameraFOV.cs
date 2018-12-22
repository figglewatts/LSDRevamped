using UnityEngine;
using System.Collections;
using Game;
using UnityEngine.VR;

[RequireComponent(typeof(Camera))]
public class SetCameraFOV : MonoBehaviour
{
	void Awake()
	{
		if (GameSettings.VR) return;

		Camera c = GetComponent<Camera>();
		c.fieldOfView = GameSettings.CurrentSettings.FOV;
	}
}
