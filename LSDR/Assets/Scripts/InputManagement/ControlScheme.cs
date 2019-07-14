using ProtoBuf;

namespace LSDR.InputManagement
{
    /// <summary>
    /// ControlScheme is a data container used to store a control scheme.
    /// It gets serialized to Google's Protobuf format.
    /// </summary>
    [ProtoContract]
    public class ControlScheme
    {
        // private member for Actions
        private readonly ControlActions _controlActions;

        /// <summary>
        /// The InControl actions of this control scheme.
        /// </summary>
        public ControlActions Actions => _controlActions;

        /// <summary>
        /// The name of this control scheme.
        /// </summary>
        [ProtoMember(1)]
        public string Name { get; set; }

        /// <summary>
        /// The encoded scheme string of this control scheme. Used to save the data.
        /// </summary>
        [ProtoMember(2)]
        public string SchemeString
        {
            get { return _controlActions.Save(); }
            set { _controlActions.Load(value); }
        }

        /// <summary>
        /// Whether or not FPS mouselook/strafing is enabled.
        /// </summary>
        [ProtoMember(3)]
        public bool FpsControls { get; set; }

        /// <summary>
        /// The mouse sensitivity of this control scheme.
        /// </summary>
        [ProtoMember(4)]
        public float MouseSensitivity { get; set; }

        /// <summary>
        /// Create the default control scheme.
        /// </summary>
        public ControlScheme() : this(new ControlActions(), "default", false) { }

        /// <summary>
        /// Create a control scheme with these parameters.
        /// </summary>
        /// <param name="actions">The bindings to use.</param>
        /// <param name="name">The name of this control scheme.</param>
        /// <param name="fpsControls">Whether or not to use FPS controls.</param>
        /// <param name="mouseSensitivity">The mouse sensitivity.</param>
        public ControlScheme(ControlActions actions, string name, bool fpsControls, float mouseSensitivity = 1)
        {
            _controlActions = actions;
            FpsControls = fpsControls;
            MouseSensitivity = mouseSensitivity;
            Name = name;
        }

        // destructor for control actions
        ~ControlScheme()
        {
            _controlActions.Destroy();
        }
    }
}
