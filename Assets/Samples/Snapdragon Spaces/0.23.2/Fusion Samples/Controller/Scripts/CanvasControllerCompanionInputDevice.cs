/******************************************************************************
 * File: CanvasControllerCompanionInputDevice.cs
 * Copyright (c) 2023 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
 *
 ******************************************************************************/

using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

public struct CanvasControllerCompanionInputDeviceState : IInputStateTypeInfo
{
    [InputControl(displayName = "Menu", name = "buttonMenu", layout = "Button", bit = 0)]
    [InputControl(displayName = "Touchpad Click", name = "touchpadClick", layout = "Button", bit = 1)]
    public int buttons;

    [InputControl(displayName = "Touchpad", name = "touchpadPosition", layout = "Vector2")]
    public Vector2 touchpadPosition;

    [InputControl(displayName = "Device Position", name = "devicePosition")]
    public Vector3 devicePosition;

    [InputControl(displayName = "Device Rotation", name = "deviceRotation")]
    public Quaternion deviceRotation;

    [InputControl(displayName = "Tracking State", name = "trackingState", layout = "Integer")]
    public int trackingState;

    public FourCC format => new FourCC('M', 'Y', 'D', 'V');
}

namespace Qualcomm.Snapdragon.Spaces.Samples
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    [InputControlLayout(stateType = typeof(CanvasControllerCompanionInputDeviceState))]
    public class CanvasControllerCompanionInputDevice : InputDevice
    {
        static CanvasControllerCompanionInputDevice()
        {
            InputSystem.RegisterLayout<CanvasControllerCompanionInputDevice>();
            foreach (InputDevice inputDevice in InputSystem.devices)
            {
                if (inputDevice is CanvasControllerCompanionInputDevice)
                {
                    return;
                }
            }

            InputSystem.AddDevice<CanvasControllerCompanionInputDevice>();
        }

        public ButtonControl buttonMenu { get; private set; }

        public ButtonControl touchpadClick { get; private set; }

        public Vector2Control touchpadPosition { get; private set; }

        public Vector3Control devicePosition { get; private set; }

        public QuaternionControl deviceRotation { get; private set; }

        public IntegerControl trackingState { get; private set; }

        protected override void FinishSetup()
        {
            base.FinishSetup();
            buttonMenu = GetChildControl<ButtonControl>("buttonMenu");
            touchpadClick = GetChildControl<ButtonControl>("touchpadClick");
            touchpadPosition = GetChildControl<Vector2Control>("touchpadPosition");
            devicePosition = GetChildControl<Vector3Control>("devicePosition");
            deviceRotation = GetChildControl<QuaternionControl>("deviceRotation");
            trackingState = GetChildControl<IntegerControl>("trackingState");
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeInPlayer() { }
    }
}
