using System;
using Entities.Action;
using Entities.Dream;
using Entities.Player;
using Entities.Trigger;
using Entities.WorldObject;
using Types;
using UnityEngine;
using Util;

namespace Entities
{
	public static class EntityInstantiator
	{
		public static GameObject Instantiate(ENTITY e)
		{
			GameObject entityObject;
			switch (e.Classname)
			{
				case "action_animate":
				{
					entityObject = AnimateAction.Instantiate(e);
					break;
				}
				case "action_move":
				{
					entityObject = MoveAction.Instantiate(e);
					break;
				}
				case "action_rotate":
				{
					entityObject = RotateAction.Instantiate(e);
					break;
				}
				case "action_sequence":
				{
					entityObject = ActionSequence.Instantiate(e);
					break;
				}
				case "action_sound":
				{
					entityObject = SoundAction.Instantiate(e);
					break;
				}
				case "action_wait":
				{
					entityObject = WaitAction.Instantiate(e);
					break;
				}
				case "audio_source":
				{
					entityObject = AudioSourceObject.Instantiate(e);
					break;
				}
				case "dream_environment":
				{
					entityObject = DreamEnvironment.Instantiate(e);
					break;
				}
				case "!map":
				{
					entityObject = MapObject.Instantiate(e);
					break;
				}
				case "!model":
				{
					entityObject = ModelObject.Instantiate(e);
					break;
				}
				case "music_controller":
				{
					entityObject = MusicController.Instantiate(e);
					break;
				}
				case "player_spawn":
				{
					entityObject = PlayerSpawn.Instantiate(e);
					break;
				}
				case "target":
				{
					entityObject = Target.Instantiate(e);
					break;
				}
				case "trigger_link":
				{
					entityObject = TriggerLink.Instantiate(e);
					break;
				}
				case "trigger_sequence":
				{
					entityObject = TriggerSequence.Instantiate(e);
					break;
				}
				case "trigger_sound":
				{
					entityObject = TriggerSound.Instantiate(e);
					break;
				}
				case "trigger_teleport":
				{
					entityObject = TriggerTeleport.Instantiate(e);
					break;
				}
				default:
				{
					Debug.LogWarning("Could not instantiate entity with classname " + e.Classname);
					entityObject = new GameObject(e.Classname);
					break;
				}
			}

			return entityObject;
		}

		public static GameObject InstantiatePrefab(string filePath, Vector3 position = default(Vector3), Quaternion rotation = default(Quaternion))
		{
			GameObject prefab = GameObject.Instantiate(Resources.Load<GameObject>(filePath));
			prefab.transform.SetParent(DreamDirector.LoadedDreamObject.transform, true);
			prefab.transform.position = position;
			prefab.transform.rotation = rotation;
			return prefab;
		}
	}
}
