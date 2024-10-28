/******************************************************************************
 * File: XRITSampleController.cs
 * Copyright (c) 2021-2022 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
 *
 ******************************************************************************/

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Qualcomm.Snapdragon.Spaces.Samples
{
    public class XRITSampleController : SampleController
    {
        public Text ScrollbarText;
        public Text TouchpadXText;
        public Text TouchpadYText;
        public Text InfoInputText;
        public Toggle HandTrackingButton;
        public Toggle GazePointerButton;
        public Toggle ControllersButton;
        public InputActionReference TouchpadInputAction;
        public RectTransform TouchpadPositionIndicator;
        private readonly float _touchpadPositionIndicatorScaleFactor = 100f;
        private bool _controllersTracked;
        private float _delayTime = 0.1f;

        public override void OnEnable()
        {
            base.OnEnable();
            InteractionManager.onInputTypeSwitch += HandleInputSwitch;
#if !QCHT_UNITY_CORE
            HandTrackingButton.gameObject.SetActive(false);
#endif
        }

        public override void OnDisable()
        {
            base.OnDisable();
            InteractionManager.onInputTypeSwitch -= HandleInputSwitch;
        }

        public override void Start()
        {
            InteractionManager.Instance.SwitchInput(InputType.ControllerPointer);
            UpdateInputInfoText(XRControllerManager._xrControllerProfile);
        }

        private void Update()
        {
            var touchpadValue = TouchpadInputAction.action.ReadValue<Vector2>();
            TouchpadXText.text = touchpadValue.x.ToString("#0.00");
            TouchpadYText.text = touchpadValue.y.ToString("#0.00");
            TouchpadPositionIndicator.anchoredPosition = touchpadValue * _touchpadPositionIndicatorScaleFactor;
            if (XRControllerManager._xrControllerProfile == XRControllerProfile.HostController)
            {
                return;
            }

#if QCHT_UNITY_CORE_4_0_0_PRE_16_OR_NEWER
            if (_controllersTracked != InteractionManager.Instance.AutomaticControllerSwitch.ControllersTracked)
            {
                if (InteractionManager.Instance.AutomaticControllerSwitch.ControllersTracked)
                {
                    HandTrackingButton.gameObject.SetActive(false);
                    ControllersButton.gameObject.SetActive(true);
                }
                else
                {
                    HandTrackingButton.gameObject.SetActive(true);
                    ControllersButton.gameObject.SetActive(false);
                }

                _controllersTracked = InteractionManager.Instance.AutomaticControllerSwitch.ControllersTracked;
            }
#endif
        }

        public void SwitchToHandTracking(bool isOn)
        {
            if (!isOn)
            {
                return;
            }

            HandleInputSwitch(InputType.HandTracking);
            Invoke(nameof(SendHandTracking), _delayTime);
        }

        private void SendHandTracking()
        {
            InteractionManager.Instance.SwitchInput(InputType.HandTracking);
        }

        public void SwitchToGazePointer(bool isOn)
        {
            if (!isOn)
            {
                return;
            }

            HandleInputSwitch(InputType.GazePointer);
            Invoke(nameof(SendGazePointer), _delayTime);
        }

        private void SendGazePointer()
        {
            InteractionManager.Instance.SwitchInput(InputType.GazePointer);
        }

        public void SwitchToControllers(bool isOn)
        {
            if (!isOn)
            {
                return;
            }

            HandleInputSwitch(InputType.ControllerPointer);
            Invoke(nameof(SendControllers), _delayTime);
        }

        private void SendControllers()
        {
            InteractionManager.Instance.SwitchInput(InputType.ControllerPointer);
        }

        private void HandleInputSwitch(InputType inputType)
        {
            switch (inputType)
            {
                case InputType.HandTracking:
                    GazePointerButton.interactable = true;
                    if (GazePointerButton.isOn)
                    {
                        GazePointerButton.isOn = false;
                    }

                    ControllersButton.interactable = true;
                    if (ControllersButton.isOn)
                    {
                        ControllersButton.isOn = false;
                    }

                    HandTrackingButton.interactable = false;
                    break;
                case InputType.GazePointer:
                    HandTrackingButton.interactable = true;
                    if (HandTrackingButton.isOn)
                    {
                        HandTrackingButton.isOn = false;
                    }

                    ControllersButton.interactable = true;
                    if (ControllersButton.isOn)
                    {
                        ControllersButton.isOn = false;
                    }

                    GazePointerButton.interactable = false;
                    break;
                case InputType.ControllerPointer:
                    HandTrackingButton.interactable = true;
                    if (HandTrackingButton.isOn)
                    {
                        HandTrackingButton.isOn = false;
                    }

                    GazePointerButton.interactable = true;
                    if (GazePointerButton.isOn)
                    {
                        GazePointerButton.isOn = false;
                    }

                    ControllersButton.interactable = false;
                    break;
            }

            if (XRControllerManager._xrControllerProfile == XRControllerProfile.HostController)
            {
                return;
            }

            switch (inputType)
            {
                case InputType.ControllerPointer:
                    HandTrackingButton.gameObject.SetActive(false);
                    ControllersButton.gameObject.SetActive(true);
                    break;
                case InputType.HandTracking:
                    ControllersButton.gameObject.SetActive(false);
                    HandTrackingButton.gameObject.SetActive(true);
                    break;
            }
        }

        public void OnScrollValueChanged(float value)
        {
            SendHapticImpulse(duration: 0.1f);
            ScrollbarText.text = value.ToString("#0.00");
        }

        private void UpdateInputInfoText(XRControllerProfile controllerProfile)
        {
            switch (controllerProfile)
            {
                case XRControllerProfile.HostController:
                    InfoInputText.text = "Switch between Hand Tracking, Gaze and Pointer controller by clicking the 'Menu' button on the controller.";
                    break;
                case XRControllerProfile.XRControllers:
                    InfoInputText.text = "Turn the controllers off for activating Hand Tracking. Use the 'Menu' button to switch between input methods.";
                    break;
            }
        }
    }
}
