using InControl;

namespace LSDR.InputManagement
{
    /// <summary>
    /// ControlActions is the InControl ActionSet used for game inputs.
    /// </summary>
    public class ControlActions : PlayerActionSet
    {
        // all possible inputs
        public PlayerAction Left;
        public PlayerAction Right;
        public PlayerAction Forward;
        public PlayerAction Backward;
        public PlayerAction LookUp;
        public PlayerAction LookDown;
        public PlayerAction LookLeft;
        public PlayerAction LookRight;
        public PlayerAction LookBehind;
        public PlayerAction Run;
        public PlayerAction Start;
        public PlayerOneAxisAction MoveX;
        public PlayerOneAxisAction MoveY;
        public PlayerOneAxisAction LookX;
        public PlayerOneAxisAction LookY;
        public PlayerTwoAxisAction Move;

        // UI inputs submit and back are bound to these inputs
        public PlayerAction Submit => Run;
        public PlayerAction Back => LookBehind;

        // set these listen options
        public static readonly BindingListenOptions DefaultListenOptions = new BindingListenOptions
        {
            IncludeMouseButtons = true,
            MaxAllowedBindings = 2,
            UnsetDuplicateBindingsOnSet = true,
            IncludeModifiersAsFirstClassKeys = true
        };

        /// <summary>
        /// Create the ControlActions with names.
        /// </summary>
        public ControlActions()
        {
            Left = CreatePlayerAction("Left");
            Right = CreatePlayerAction("Right");
            Forward = CreatePlayerAction("Forward");
            Backward = CreatePlayerAction("Backward");
            LookUp = CreatePlayerAction("Look up");
            LookDown = CreatePlayerAction("Look down");
            LookLeft = CreatePlayerAction("Look left");
            LookRight = CreatePlayerAction("Look right");
            LookBehind = CreatePlayerAction("Look behind / UI Return");
            Run = CreatePlayerAction("Run / UI Select");
            Start = CreatePlayerAction("Start");
            MoveX = CreateOneAxisPlayerAction(Left, Right);
            MoveY = CreateOneAxisPlayerAction(Backward, Forward);
            LookX = CreateOneAxisPlayerAction(LookLeft, LookRight);
            LookY = CreateOneAxisPlayerAction(LookDown, LookUp);
            Move = CreateTwoAxisPlayerAction(Left, Right, Backward, Forward);

            MoveX.StateThreshold = 0.5f;
            MoveY.StateThreshold = 0.5f;

            ListenOptions = DefaultListenOptions;
        }

        /// <summary>
        /// The default bindings for tank (classic) controls.
        /// </summary>
        /// <returns>Default bindings for tank controls.</returns>
        public static ControlActions CreateDefaultTank()
        {
            ControlActions actions = new ControlActions();

            actions.Left.AddDefaultBinding(Key.A);
            actions.Left.AddDefaultBinding(InputControlType.LeftStickLeft);
            actions.Left.AddDefaultBinding(InputControlType.DPadLeft);

            actions.Right.AddDefaultBinding(Key.D);
            actions.Right.AddDefaultBinding(InputControlType.LeftStickRight);
            actions.Right.AddDefaultBinding(InputControlType.DPadRight);

            actions.Forward.AddDefaultBinding(Key.W);
            actions.Forward.AddDefaultBinding(InputControlType.LeftStickUp);
            actions.Forward.AddDefaultBinding(InputControlType.DPadUp);

            actions.Backward.AddDefaultBinding(Key.S);
            actions.Backward.AddDefaultBinding(InputControlType.LeftStickDown);
            actions.Backward.AddDefaultBinding(InputControlType.DPadDown);

            actions.LookUp.AddDefaultBinding(Key.E);
            actions.LookUp.AddDefaultBinding(InputControlType.Action4);

            actions.LookDown.AddDefaultBinding(Key.Q);
            actions.LookDown.AddDefaultBinding(InputControlType.Action3);

            actions.LookBehind.AddDefaultBinding(Key.Z);
            actions.LookBehind.AddDefaultBinding(InputControlType.Action2);

            actions.Run.AddDefaultBinding(Key.Space);
            actions.Run.AddDefaultBinding(InputControlType.Action1);
            
            actions.Start.AddDefaultBinding(Key.Escape);
            actions.Start.AddDefaultBinding(InputControlType.Start);

            return actions;
        }

        /// <summary>
        /// Create default bindings for FPS controls.
        /// </summary>
        /// <returns>Default bindings for FPS controls.</returns>
        public static ControlActions CreateDefaultFps()
        {
            ControlActions actions = new ControlActions();

            actions.Left.AddDefaultBinding(Key.A);
            actions.Left.AddDefaultBinding(InputControlType.LeftStickLeft);
            actions.Left.AddDefaultBinding(InputControlType.DPadLeft);

            actions.Right.AddDefaultBinding(Key.D);
            actions.Right.AddDefaultBinding(InputControlType.LeftStickRight);
            actions.Right.AddDefaultBinding(InputControlType.DPadRight);

            actions.Forward.AddDefaultBinding(Key.W);
            actions.Forward.AddDefaultBinding(InputControlType.LeftStickUp);
            actions.Forward.AddDefaultBinding(InputControlType.DPadUp);

            actions.Backward.AddDefaultBinding(Key.S);
            actions.Backward.AddDefaultBinding(InputControlType.LeftStickDown);
            actions.Backward.AddDefaultBinding(InputControlType.DPadDown);

            actions.LookUp.AddDefaultBinding(Mouse.PositiveY);
            actions.LookUp.AddDefaultBinding(InputControlType.RightStickUp);

            actions.LookDown.AddDefaultBinding(Mouse.NegativeY);
            actions.LookDown.AddDefaultBinding(InputControlType.RightStickDown);

            actions.LookLeft.AddDefaultBinding(Mouse.NegativeX);
            actions.LookLeft.AddDefaultBinding(InputControlType.RightStickLeft);

            actions.LookRight.AddDefaultBinding(Mouse.PositiveX);
            actions.LookRight.AddDefaultBinding(InputControlType.RightStickRight);

            actions.LookBehind.AddDefaultBinding(Key.Z);
            actions.LookBehind.AddDefaultBinding(InputControlType.Action2);

            actions.Run.AddDefaultBinding(Key.Space);
            actions.Run.AddDefaultBinding(InputControlType.Action1);
            
            actions.Start.AddDefaultBinding(Key.Escape);
            actions.Start.AddDefaultBinding(InputControlType.Start);

            return actions;
        }
    }
}