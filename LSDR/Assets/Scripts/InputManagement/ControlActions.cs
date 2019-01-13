using System.Collections;
using System.Collections.Generic;
using InControl;
using UnityEngine;

namespace InputManagement
{
    public class ControlActions : PlayerActionSet
    {
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
        public PlayerOneAxisAction MoveX;
        public PlayerOneAxisAction MoveY;
        public PlayerOneAxisAction LookX;
        public PlayerOneAxisAction LookY;
        public PlayerTwoAxisAction Move;

        public PlayerAction Submit => Run;
        public PlayerAction Back => LookBehind;

        public static readonly BindingListenOptions DefaultListenOptions = new BindingListenOptions
        {
            IncludeMouseButtons = true,
            MaxAllowedBindings = 2,
            UnsetDuplicateBindingsOnSet = true,
            IncludeModifiersAsFirstClassKeys = true
        };

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
            MoveX = CreateOneAxisPlayerAction(Left, Right);
            MoveY = CreateOneAxisPlayerAction(Backward, Forward);
            LookX = CreateOneAxisPlayerAction(LookLeft, LookRight);
            LookY = CreateOneAxisPlayerAction(LookDown, LookUp);
            Move = CreateTwoAxisPlayerAction(Left, Right, Backward, Forward);

            ListenOptions = DefaultListenOptions;
        }

        public static ControlActions CreateDefaultTank()
        {
            ControlActions actions = new ControlActions();

            actions.Left.AddDefaultBinding(Key.A);
            actions.Left.AddDefaultBinding(InputControlType.LeftStickLeft);

            actions.Right.AddDefaultBinding(Key.D);
            actions.Right.AddDefaultBinding(InputControlType.LeftStickRight);

            actions.Forward.AddDefaultBinding(Key.W);
            actions.Forward.AddDefaultBinding(InputControlType.LeftStickUp);

            actions.Backward.AddDefaultBinding(Key.S);
            actions.Backward.AddDefaultBinding(InputControlType.LeftStickDown);

            actions.LookUp.AddDefaultBinding(Key.E);
            actions.LookUp.AddDefaultBinding(InputControlType.Action4);

            actions.LookDown.AddDefaultBinding(Key.Q);
            actions.LookDown.AddDefaultBinding(InputControlType.Action3);

            actions.LookBehind.AddDefaultBinding(Key.Z);
            actions.LookBehind.AddDefaultBinding(InputControlType.Action2);

            actions.Run.AddDefaultBinding(Key.Space);
            actions.Run.AddDefaultBinding(InputControlType.Action2);

            return actions;
        }

        public static ControlActions CreateDefaultFps()
        {
            ControlActions actions = new ControlActions();

            actions.Left.AddDefaultBinding(Key.A);
            actions.Left.AddDefaultBinding(InputControlType.LeftStickLeft);

            actions.Right.AddDefaultBinding(Key.D);
            actions.Right.AddDefaultBinding(InputControlType.LeftStickRight);

            actions.Forward.AddDefaultBinding(Key.W);
            actions.Forward.AddDefaultBinding(InputControlType.LeftStickUp);

            actions.Backward.AddDefaultBinding(Key.S);
            actions.Backward.AddDefaultBinding(InputControlType.LeftStickDown);

            actions.LookUp.AddDefaultBinding(Mouse.PositiveY);
            actions.LookUp.AddDefaultBinding(InputControlType.RightStickUp);

            actions.LookDown.AddDefaultBinding(Mouse.NegativeY);
            actions.LookDown.AddDefaultBinding(InputControlType.RightStickDown);

            actions.LookLeft.AddDefaultBinding(Mouse.NegativeX);
            actions.LookLeft.AddDefaultBinding(InputControlType.RightStickLeft);

            actions.LookRight.AddDefaultBinding(Mouse.PositiveX);
            actions.LookRight.AddDefaultBinding(InputControlType.RightStickRight);

            actions.Run.AddDefaultBinding(Key.Space);
            actions.Run.AddDefaultBinding(InputControlType.Action2);

            return actions;
        }
    }
}