using UnityEngine;
using System.Collections;
using Game;

[RequireComponent(typeof(Camera))]
public class SetCameraFOV : MonoBehaviour
{
	void Awake()
	{
		Camera c = GetComponent<Camera>();
		c.fieldOfView = GameSettings.FOV;
	}
}
