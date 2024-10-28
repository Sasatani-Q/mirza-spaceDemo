// /******************************************************************************
//  * File: LookAtUserGrabInteractable.cs
//  * Copyright (c) 2023 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
//  *
//  *
//  ******************************************************************************/

using QCHT.Interactions.Core;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace QCHT.Samples.XRKeyboard
{
    public class LookAtUserGrabInteractable : XRGrabInteractable
    {
        private Transform _userTransform;
        private bool _shouldFaceUser;
        private float _xStartingAngle;

        protected override void OnEnable()
        {
            base.OnEnable();
            
            selectEntered.AddListener(EnableLookAtUser);
            selectExited.AddListener(DisableLookAtUser);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            selectEntered.RemoveListener(EnableLookAtUser);
            selectExited.RemoveListener(DisableLookAtUser);
        }
        
        private void Start()
        {
            _userTransform = XROriginUtility.GetOriginCamera().transform;
            _xStartingAngle = transform.eulerAngles.x;
        }

        private void Update()
        {
            if (!_shouldFaceUser)
                return;

            transform.LookAt(_userTransform.position);
        }

        public void EnableLookAtUser(SelectEnterEventArgs _)
        {
            _shouldFaceUser = true;
        }

        public void DisableLookAtUser(SelectExitEventArgs _)
        {
            _shouldFaceUser = false;
        }
    }
}