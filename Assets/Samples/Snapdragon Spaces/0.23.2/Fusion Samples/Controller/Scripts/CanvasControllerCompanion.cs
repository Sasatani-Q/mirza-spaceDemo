/******************************************************************************
 * File: CanvasControllerCompanion.cs
 * Copyright (c) 2023 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
 *
 ******************************************************************************/

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.XR.Management;

namespace Qualcomm.Snapdragon.Spaces.Samples
{
    public class CanvasControllerCompanion : MonoBehaviour
    {
        public UnityEvent OnTouchpadEnd;

        private CanvasControllerCompanionInputDeviceState deviceState;
        private CanvasControllerCompanionInputDevice inputDevice;

        private void Awake()
        {
            deviceState = new CanvasControllerCompanionInputDeviceState();
            deviceState.trackingState = 1;
        }

        public void SendTouchpadEvent(int phase, Vector2 position)
        {
            var bit = 1 << 1;
            if (phase != 0)
            {
                deviceState.buttons |= (ushort)bit;
            }
            else
            {
                deviceState.buttons &= (ushort)~bit;
                OnTouchpadEnd?.Invoke();
            }

            deviceState.touchpadPosition.x = position.x;
            deviceState.touchpadPosition.y = position.y;

            InputSystem.QueueStateEvent(InputSystem.GetDevice<CanvasControllerCompanionInputDevice>(), deviceState);
        }

        public void SendMenuButtonEvent(int phase)
        {
            var bit = 1 << 0;
            if (phase != 0)
            {
                deviceState.buttons |= (ushort)bit;
            }
            else
            {
                deviceState.buttons &= (ushort)~bit;
            }

            InputSystem.QueueStateEvent(InputSystem.GetDevice<CanvasControllerCompanionInputDevice>(), deviceState);
        }

        public void Quit()
        {
            XRGeneralSettings.Instance.Manager.DeinitializeLoader();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
