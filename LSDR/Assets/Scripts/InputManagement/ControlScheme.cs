using System;
using System.Collections.Generic;
using System.Text;
using Game;
using Newtonsoft.Json;
using ProtoBuf;
using SimpleJSON;
using UnityEngine;
using Util;

namespace InputManagement
{
    [ProtoContract]
    public class ControlScheme
    {
        private readonly ControlActions _controlActions;

        public ControlActions Actions => _controlActions;

        [ProtoMember(1)]
        public string Name { get; set; }

        [ProtoMember(2)]
        public string SchemeString
        {
            get { return _controlActions.Save(); }
            set { _controlActions.Load(value); }
        }

        [ProtoMember(3)]
        public bool FpsControls { get; set; }

        [ProtoMember(4)]
        public float MouseSensitivity { get; set; }

        public ControlScheme() : this(new ControlActions(), "default", false) { }

        public ControlScheme(ControlActions actions, string name, bool fpsControls, float mouseSensitivity = 1)
        {
            _controlActions = actions;
            FpsControls = fpsControls;
            MouseSensitivity = mouseSensitivity;
            Name = name;
        }

        ~ControlScheme()
        {
            _controlActions.Destroy();
        }
    }
}
