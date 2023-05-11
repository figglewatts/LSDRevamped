using ProtoBuf;
using UnityEngine.InputSystem;

namespace LSDR.InputManagement
{
    /// <summary>
    ///     ControlScheme is a data container used to store a control scheme.
    ///     It gets serialized to Google's Protobuf format.
    /// </summary>
    [ProtoContract(SkipConstructor = true)]
    public class ControlScheme
    {
        /// <summary>
        ///     Create a control scheme with these parameters.
        /// </summary>
        /// <param name="name">The name of this control scheme.</param>
        /// <param name="inputActions">The InputActions currently in use.</param>
        /// <param name="fpsControls">Whether or not to use FPS controls.</param>
        /// <param name="mouseSensitivity">The mouse sensitivity.</param>
        /// <param name="invertLookY">Whether or not to invert the Look Y vector</param>
        /// <param name="editable">Whether or not this control scheme is editable in the UI</param>
        public ControlScheme(string name,
            InputActions inputActions,
            bool fpsControls,
            float mouseSensitivity,
            bool invertLookY,
            bool editable)
        {
            Name = name;
            SyncToInputActions(inputActions);
            FpsControls = fpsControls;
            MouseSensitivity = mouseSensitivity;
            InvertLookY = invertLookY;
            Editable = editable;
        }

        public ControlScheme(ControlScheme other)
        {
            Name = "";
            FpsControls = other.FpsControls;
            MouseSensitivity = other.MouseSensitivity;
            InvertLookY = other.InvertLookY;
        }

        /// <summary>
        ///     The name of this control scheme.
        /// </summary>
        [ProtoMember(1)]
        public string Name { get; set; }

        /// <summary>
        ///     The encoded scheme string of this control scheme. Used to save the data.
        /// </summary>
        [ProtoMember(2)]
        public string SchemeString { get; set; } = "";

        /// <summary>
        ///     Whether or not FPS mouselook/strafing is enabled.
        /// </summary>
        [ProtoMember(3)]
        public bool FpsControls { get; set; }

        /// <summary>
        ///     The mouse sensitivity of this control scheme.
        /// </summary>
        [ProtoMember(4)]
        public float MouseSensitivity { get; set; }

        /// <summary>
        ///     Whether or not to invert the Y direction of the Look vector.
        /// </summary>
        [ProtoMember(5)]
        public bool InvertLookY { get; set; }

        /// <summary>
        ///     Whether or not this control scheme is editable in the UI.
        /// </summary>
        [ProtoMember(6)]
        public bool Editable { get; set; } = true;

        public void SyncToInputActions(InputActions inputActions)
        {
            SchemeString = inputActions.SaveBindingOverridesAsJson();
        }
    }
}
