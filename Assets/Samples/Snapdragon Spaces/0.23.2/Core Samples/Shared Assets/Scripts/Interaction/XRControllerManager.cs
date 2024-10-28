/******************************************************************************
 * File: XRControllerManager.cs
 * Copyright (c) 2022 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
 *
 ******************************************************************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.OpenXR.Input;
using InputDevice = UnityEngine.XR.InputDevice;

namespace Qualcomm.Snapdragon.Spaces.Samples
{
    public enum ControllerHand
    {
        LeftController = 0,
        RightController = 1
    }

    public enum XRControllerProfile
    {
        HostController = 0,
        XRControllers = 1
    }

    public class XRControllerManager : MonoBehaviour
    {
        public GameObject HostController;
        public GameObject XRControllers;
        public InputActionReference HostHapticInputAction;
        public InputActionReference LeftControllerHapticInputAction;
        public InputActionReference RightControllerHapticInputAction;
        public XRControllerProfile _xrControllerProfile { get; private set; }

        public void OnEnable()
        {
            InputDevices.deviceConnected += RegisterConnectedDevice;
            RegisterXRProfiles();
        }

        public void OnDisable()
        {
            InputDevices.deviceDisconnected -= RegisterConnectedDevice;
        }

        public void Start()
        {
            ActivateController(_xrControllerProfile);
        }

        public void ActivateController(XRControllerProfile xrControllerProfile)
        {
            _xrControllerProfile = xrControllerProfile;
            switch (xrControllerProfile)
            {
                case XRControllerProfile.XRControllers:
                {
                    HostController.SetActive(false);
                    XRControllers.SetActive(true);
                    break;
                }
                default:
                {
                    XRControllers.SetActive(false);
                    HostController.SetActive(true);
                    break;
                }
            }
        }

        private void RegisterXRProfiles()
        {
            List<InputDevice> inputDevices = new List<InputDevice>();
            InputDevices.GetDevices(inputDevices);
            foreach (var inputDevice in inputDevices)
            {
                RegisterConnectedDevice(inputDevice);
            }
        }

        private void RegisterConnectedDevice(InputDevice inputDevice)
        {
            _xrControllerProfile = inputDevice.name.Contains("Oculus") ? XRControllerProfile.XRControllers : XRControllerProfile.HostController;
        }

        public void SendHapticImpulse(float amplitude = 0.5f, float frequency = 60f, float duration = 0.1f, ControllerHand controllerHand = ControllerHand.LeftController)
        {
            switch (_xrControllerProfile)
            {
                case XRControllerProfile.HostController:
                    OpenXRInput.SendHapticImpulse(HostHapticInputAction, amplitude, frequency, duration);
                    break;
                case XRControllerProfile.XRControllers:
                    switch (controllerHand)
                    {
                        case ControllerHand.LeftController:
                            OpenXRInput.SendHapticImpulse(LeftControllerHapticInputAction, amplitude, frequency, duration, UnityEngine.InputSystem.XR.XRController.leftHand);
                            break;
                        case ControllerHand.RightController:
                            OpenXRInput.SendHapticImpulse(RightControllerHapticInputAction, amplitude, frequency, duration, UnityEngine.InputSystem.XR.XRController.rightHand);
                            break;
                    }
                    break;
            }
        }
    }
}
