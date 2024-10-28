// /******************************************************************************
//  * File: PassthroughActivator.cs
//  * Copyright (c) 2024 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
//  *
//  *
//  ******************************************************************************/

using QCHT.Interactions.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace QCHT.Samples.Menu
{
    public class PassthroughActivator : MonoBehaviour
    {
        [SerializeField] private bool enableOnStart;
        
        [SerializeField] private InputActionProperty togglePassthroughAction;

        private void OnEnable()
        {
            togglePassthroughAction.EnableDirectAction();

            togglePassthroughAction.action.performed += TogglePassthrough;
        }

        private void OnDisable()
        {
            togglePassthroughAction.DisableDirectAction();

            togglePassthroughAction.action.performed -= TogglePassthrough;
        }

        private void Start()
        {
            TogglePassthrough(enableOnStart || XRPassthroughUtility.GetPassthroughEnabled());
        }
        
        private void TogglePassthrough(InputAction.CallbackContext _) => TogglePassthrough();

        public void TogglePassthrough()
        {
            var enable = XRPassthroughUtility.GetPassthroughEnabled();
            enable = !enable;
            TogglePassthrough(enable);
        }

        public void TogglePassthrough(bool enable) => XRPassthroughUtility.SetPassthroughEnabled(enable);
    }
}