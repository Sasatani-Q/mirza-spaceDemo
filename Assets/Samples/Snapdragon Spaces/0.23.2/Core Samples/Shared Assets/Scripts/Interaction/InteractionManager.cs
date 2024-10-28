/******************************************************************************
 * File: InteractionManager.cs
 * Copyright (c) 2023 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
 *
 ******************************************************************************/

using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.XR.OpenXR;
#if QCHT_UNITY_CORE
using QCHT.Interactions.Core;
#endif

#if AR_FOUNDATION_5_0_OR_NEWER
using Unity.XR.CoreUtils;
#endif

namespace Qualcomm.Snapdragon.Spaces.Samples
{
    public enum InputType
    {
        HandTracking = 0,
        GazePointer = 1,
        ControllerPointer = 2
    }

    public class InteractionManager : MonoBehaviour
    {
        public static InteractionManager Instance { get; private set; }

        public delegate void OnInputTypeSwitch(InputType inputType);

        private const string _controllerTypePrefsKey = "Qualcomm.Snapdragon.Spaces.Samples.Prefs.ControllerType";
        public static OnInputTypeSwitch onInputTypeSwitch;
        public GameObject HandTrackingPointer;
        public GameObject GazePointer;
        public GameObject DevicePointer;
        public InputActionReference SwitchInputAction;
#if QCHT_UNITY_CORE_4_0_0_PRE_16_OR_NEWER
        public AutomaticControllerSwitch AutomaticControllerSwitch;
#endif
        [SerializeField]
        private XRControllerManager _xrControllerManager;
        private bool _isHandTrackingCompatible;
        private bool _isSessionOriginMoved;
#if QCHT_UNITY_CORE
        public XRHandTrackingManager HandTrackingManager { get; private set; }
#if QCHT_UNITY_CORE_4_1_5_OR_NEWER
        public QCHT.Interactions.Core.XRHandTrackingSubsystem HandTrackingSubsystem { get; private set; }
#endif
#endif
        public InputType InputType { get; private set; }
        public Transform ArCameraTransform { get; private set; }
        protected virtual bool ResetSessionOriginOnStart => true;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.LogWarning($"Multiple InteractionManager components cannot exist in the same scene. Destroying InteractionManager with InstanceID {gameObject.GetInstanceID()}");
                Destroy(this);
            }

            Instance = this;
        }

        public void Start()
        {
            ArCameraTransform = OriginLocationUtility.GetOriginCamera().transform;
            int controllerType = PlayerPrefs.GetInt(_controllerTypePrefsKey, 0);
            SetControllerProfileType((InputType)controllerType);
        }

        public void Update()
        {
            if (ResetSessionOriginOnStart && !_isSessionOriginMoved && ArCameraTransform.position != Vector3.zero)
            {
                OffsetSessionOrigin();
                _isSessionOriginMoved = true;
            }
        }

        public void OnEnable()
        {
            SwitchInputAction.action.performed += OnSwitchInput;

            _isHandTrackingCompatible = IsHandTrackingCompatible();
            if (_isHandTrackingCompatible)
            {
#if QCHT_UNITY_CORE
                HandTrackingSubsystem ??= QCHT.Interactions.Core.XRHandTrackingSubsystem.GetSubsystemInManager();
                HandTrackingManager = XRHandTrackingManager.InstantiateHandTrackingManager();
#endif
            }
        }

        public void OnDisable()
        {
            SwitchInputAction.action.performed -= OnSwitchInput;
#if QCHT_UNITY_CORE_4_1_5_OR_NEWER
            HandTrackingSubsystem?.Stop();
#endif
        }

        /// <summary>
        ///     Switch to the next input modality.
        /// </summary>
        public void SwitchInput()
        {
            var newInputType = InputType + 1;
            var inputTypeCount = Enum.GetNames(typeof(InputType)).Length;
            if (newInputType < 0 || (int)newInputType >= inputTypeCount)
            {
                newInputType = 0;
            }
#if QCHT_UNITY_CORE_4_0_0_PRE_16_OR_NEWER
            if (AutomaticControllerSwitch.ControllersTracked && GetControllerProfile() == XRControllerProfile.XRControllers)
            {
                newInputType = newInputType == InputType.HandTracking ? InputType.GazePointer : newInputType;
            }
#endif
            SetControllerProfileType(newInputType);
        }

        /// <summary>
        ///     Switch to a specific input modality.
        /// </summary>
        /// <param name="NewInputType"></param>
        public void SwitchInput(InputType NewInputType)
        {
            SetControllerProfileType(NewInputType);
        }

        /// <summary>
        ///     Quit application.
        /// </summary>
        public void Quit()
        {
            SendHapticImpulse();
            Application.Quit();
        }

        /// <summary>
        ///     Send haptic impulse to controllers.
        /// </summary>
        /// <param name="amplitude"></param>
        /// <param name="frequency"></param>
        /// <param name="duration"></param>
        public void SendHapticImpulse(float amplitude = 0.5f, float frequency = 60f, float duration = 0.1f)
        {
            if (InputType == InputType.ControllerPointer)
            {
                _xrControllerManager.SendHapticImpulse(amplitude, frequency, duration);
            }
        }

        private void OnSwitchInput(InputAction.CallbackContext ctx)
        {
            if (ctx.interaction is TapInteraction)
            {
                SwitchInput();
                ResetPointerPose();
            }
        }

        private void SetControllerProfileType(InputType inputType)
        {
            // Checks if QCHT package is installed. If not, the Gaze Pointer will be the fallback.
            if (!_isHandTrackingCompatible)
            {
                InputType = inputType != InputType.HandTracking ? inputType : InputType.GazePointer;
            }
            else
            {
                InputType = inputType;
            }

            // Activates the Pointer used for interaction.
            switch (InputType)
            {
                case InputType.HandTracking:
                {
                    HandleHandTrackingDevices(true);
                    HandTrackingPointer.SetActive(true);
                    GazePointer.SetActive(false);
                    DevicePointer.SetActive(false);
                    break;
                }
                case InputType.GazePointer:
                {
                    HandTrackingPointer.SetActive(false);
                    HandleHandTrackingDevices(false);
                    GazePointer.SetActive(true);
                    DevicePointer.SetActive(false);
                    break;
                }
                case InputType.ControllerPointer:
                {
                    HandTrackingPointer.SetActive(false);
                    HandleHandTrackingDevices(false);
                    GazePointer.SetActive(false);
                    DevicePointer.SetActive(true);
                    break;
                }
            }

            // Sets the pointer type and saves it in the PlayerPrefs.
            int pointerType = GazePointer.activeSelf ? (int)InputType.GazePointer :
                DevicePointer.activeSelf ? (int)InputType.ControllerPointer :
                HandTrackingPointer.activeSelf ? (int)InputType.HandTracking : 0;
            PlayerPrefs.SetInt(_controllerTypePrefsKey, pointerType);
            onInputTypeSwitch?.Invoke(InputType);
        }

        private void ResetPointerPose()
        {
            var baseRuntimeFeature = OpenXRSettings.Instance.GetFeature<BaseRuntimeFeature>();
            if (baseRuntimeFeature)
            {
                baseRuntimeFeature.TryResetPose();
            }
        }

        // Enables or disables Hand Tracking depending on the Input mode is in use.
        private void HandleHandTrackingDevices(bool enable)
        {
#if QCHT_UNITY_CORE
            if (enable)
            {
                ToggleHands(true);
#if QCHT_UNITY_CORE_4_1_5_OR_NEWER
                HandTrackingSubsystem?.Start();
#else
                HandTrackingManager.StartHandTracking();
#endif
            }
            else
            {

                ToggleHands(false);
#if QCHT_UNITY_CORE_4_1_5_OR_NEWER
                HandTrackingSubsystem?.Stop();
#else
                HandTrackingManager.StopHandTracking();
#endif
            }
        }

        private void ToggleHands(bool enabled)
        {
            HandTrackingManager.ToggleLeftHand(enabled);
            HandTrackingManager.ToggleRightHand(enabled);
        }
#endif

#if QCHT_UNITY_CORE_4_0_0_PRE_16_OR_NEWER
        public HandTrackingStatus GetHandTrackingStatus()
        {
#if QCHT_UNITY_CORE_4_1_5_OR_NEWER
            return HandTrackingSubsystem.Status;
#else
            return HandTrackingManager.HandTrackingStatus;
#endif
#endif
        }

        protected void OffsetSessionOrigin()
        {
            var sessionOrigin = OriginLocationUtility.GetOriginTransform();
            sessionOrigin.Rotate(0.0f, -ArCameraTransform.rotation.eulerAngles.y, 0.0f, Space.World);
            sessionOrigin.position = -ArCameraTransform.position;
        }

        public XRControllerProfile GetControllerProfile()
        {
            return _xrControllerManager._xrControllerProfile;
        }

        // Checks if QCHT package is in the project.
        private bool IsHandTrackingCompatible()
        {
#if QCHT_UNITY_CORE
            // Return ture only if QCHT package is in the project and create XR Hand Tracking Manager.
            return true;
#endif
#if !QCHT_UNITY_CORE
            return false;
#endif
        }
    }
}
