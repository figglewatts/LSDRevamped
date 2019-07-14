using System.Collections.Generic;
using LSDR.Types;
using UnityEngine;
using LSDR.Util;

namespace LSDR.Entities.Action
{
	// TODO: ActionSequence is obsolete
	public class ActionSequence : MonoBehaviour
	{
		public static List<ActionSequence> Sequences = new List<ActionSequence>();
	
		public string SequenceName;
		public string NextSequence;
		public int TimesToLoop;
		public bool BeginOnLevelLoad;
		public bool LoopSequence;
		public bool HasNextSequence;

		public GameObject ReferencedGameObject;

		public List<BaseAction> ActionsInSequence = new List<BaseAction>();

		private ActionSequence _referencedNextSequence;

		private int _currentAction = 0;
		private int _timesLooped = 0;

		public void Start()
		{
			if (BeginOnLevelLoad) BeginSequence();
			if (HasNextSequence)
			{
				_referencedNextSequence = FindSequence(NextSequence);
				if (!_referencedNextSequence.ReferencedGameObject)
					_referencedNextSequence.ReferencedGameObject = ReferencedGameObject;
			}
		}

		public void BeginSequence()
		{
			_currentAction = 0;
			StartCoroutine(ActionsInSequence[_currentAction].DoAction());
		}

		public void DoNextAction()
		{
			_currentAction++;
			if (_currentAction >= ActionsInSequence.Count)
			{
				if (LoopSequence && (TimesToLoop == 0 || _timesLooped < TimesToLoop))
				{
					_currentAction = 0;
					_timesLooped++;
				}
				else if (HasNextSequence)
				{
					FindSequence(NextSequence).BeginSequence();
					return;
				}
				else return;
			}
			StartCoroutine(ActionsInSequence[_currentAction].DoAction());
		}

		public void AddAction(BaseAction a, int index)
		{
			if (index > ActionsInSequence.Count)
			{
				int originalCount = ActionsInSequence.Count;
				for (int i = originalCount; i < index; i++)
				{
					ActionsInSequence.Add(null);
				}
			}
			ActionsInSequence.Insert(index, a);
			ActionsInSequence.RemoveAll(item => item == null); // remove null elements
		}

		public static GameObject Instantiate(ENTITY e)
		{
			GameObject instantiated = new GameObject(e.Classname);
			ActionSequence actionSequence = instantiated.AddComponent<ActionSequence>();

			actionSequence.SequenceName = e.GetPropertyValue("Sequence name");
			if (actionSequence.SequenceName.Equals(string.Empty))
			{
				Debug.Log("Found action_sequence without name! Please set name in Torii.");
			}
			actionSequence.NextSequence = e.GetPropertyValue("Next sequence");

			actionSequence.TimesToLoop = EntityUtil.TryParseInt("Times to loop", e);

			actionSequence.BeginOnLevelLoad = e.GetSpawnflagValue(0, 3);
			actionSequence.LoopSequence = e.GetSpawnflagValue(1, 3);
			actionSequence.HasNextSequence = e.GetSpawnflagValue(2, 3);

			Sequences.Add(actionSequence);

			EntityUtil.SetInstantiatedObjectTransform(e, ref instantiated);

			return instantiated;
		}

		public static ActionSequence FindSequence(string sequenceName)
		{
			foreach (ActionSequence seq in Sequences)
			{
				if (seq.SequenceName.Equals(sequenceName)) return seq;
			}
			Debug.LogWarning("Could not find sequence with name: " + sequenceName + ", please fix in Torii");
			return null;
		}
	}
}
