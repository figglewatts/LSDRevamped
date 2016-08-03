using System;
using System.Collections.Generic;
using System.Text;
using Game;
using SimpleJSON;
using UnityEngine;
using Util;

namespace InputManagement
{
	public class ControlScheme
	{
		public readonly Dictionary<string, KeyCode> Controls = new Dictionary<string, KeyCode>();
		public bool FPSMovementEnabled;
		public string SchemeName;

		public float MouseSensitivity;

		/// <summary>
		/// Creates a control scheme with the default settings
		/// </summary>
		public ControlScheme()
		{
			Controls.Add("Forward", KeyCode.W);
			Controls.Add("Left", KeyCode.A);
			Controls.Add("Backward", KeyCode.S);
			Controls.Add("Right", KeyCode.D);
			Controls.Add("LookUp", KeyCode.Q);
			Controls.Add("LookDown", KeyCode.E);
			Controls.Add("Sprint", KeyCode.Space);
			FPSMovementEnabled = false;
			SchemeName = "Classic";
			MouseSensitivity = 1F;
		}

		/// <summary>
		/// Loads a control scheme from a JSON file
		/// </summary>
		/// <param name="jsonFile">Path to the file</param>
		public ControlScheme(string jsonFile)
		{
			JSONClass json = ResourceManager.Load<JSONClass>(jsonFile, ResourceLifespan.GLOBAL, true);
			foreach (JSONNode n in json["controls"].AsArray)
			{
				Controls.Add(n["name"], (KeyCode) n["keyCode"].AsInt);
			}
			FPSMovementEnabled = json["fpsMovementEnabled"].AsBool;
			SchemeName = json["name"];
			MouseSensitivity = json["mouseSensitivity"].AsFloat;
		}

		/// <summary>
		/// Gets the JSON representation of this control scheme
		/// </summary>
		public JSONClass SerializeToJSON()
		{
			JSONClass json = new JSONClass();
			int i = 0;
			foreach (string key in Controls.Keys)
			{
				json["controls"][i]["name"] = key;
				json["controls"][i]["keyCode"].AsInt = (int) Controls[key];
				i++;
			}
			json["fpsMovementEnabled"].AsBool = FPSMovementEnabled;
			json["name"] = SchemeName;
			json["mouseSensitivity"].AsFloat = MouseSensitivity;
			return json;
		}
	}
}
