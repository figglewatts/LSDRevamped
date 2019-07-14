using UnityEngine;
using LSDR.Types;

namespace LSDR.Entities.WorldObject
{
	/// <summary>
	/// Used to access TOBJ struct on instantiated GameObjects
	/// </summary>
	// TODO: ToriiObject is now obsolete
	public class ToriiObject : MonoBehaviour
	{
		public TOBJ ToriiObj;
	}
}
