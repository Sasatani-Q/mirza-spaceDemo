// /******************************************************************************
//  * File: RayInteractorManagerKeyboard.cs
//  * Copyright (c) 2023 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
//  *
//  *
//  ******************************************************************************/

using QCHT.Interactions.Core;
using QCHT.Interactions.Hands;
using QCHT.Interactions.Distal;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace QCHT.Samples.XRKeyboard
{
    public class RayInteractorManagerKeyboard : XRRayInteractorManager
    {
        private Transform _leftHand, _rightHand;
        [SerializeField] private float distanceToDesactivate = 0.5f;
        private Transform _keyboardTransform = null;

        public Transform KeyboardTransform
        {
            get => _keyboardTransform;
            set => _keyboardTransform = value;
        }

        [SerializeField] private bool _shouldHideHands = false;

        private bool _shouldShowRightHandsAndRays = true;
        private bool _shouldShowLeftHandsAndRays = true;

        private void Start()
        {
            _leftHand = XRHandTrackingManager.GetOrCreate().LeftHand.transform;
            _rightHand = XRHandTrackingManager.GetOrCreate().RightHand.transform;
        }
        
        protected override void Update()
        {
            // if (!updateRays) return;

            //base.Update();
            var currentShowRight = _shouldShowRightHandsAndRays;
            var currentShowLeft = _shouldShowLeftHandsAndRays;

            if (_keyboardTransform == null)
                return;
            var keyboardPosition = _keyboardTransform.position;
            _shouldShowRightHandsAndRays =
                !(Vector3.Distance(keyboardPosition, _rightHand.position) <= distanceToDesactivate);
            _shouldShowLeftHandsAndRays =
                !(Vector3.Distance(keyboardPosition, _leftHand.position) <= distanceToDesactivate);

            // leftController.SetActive(_shouldShowLeftHandsAndRays && leftIsTracked.IsInProgress());
            // rightController.SetActive(_shouldShowRightHandsAndRays && rightIsTracked.IsInProgress());

            if (_shouldHideHands)
            {
                //Hide hands
                if (currentShowLeft != _shouldShowLeftHandsAndRays)
                {
                    if (_leftHand.TryGetComponent(out XRBaseController xrController) && xrController.model != null &&
                        xrController.model.TryGetComponent(out IHideable hideable))
                    {
                        if (_shouldShowLeftHandsAndRays)
                            hideable.Show();
                        else
                            hideable.Hide();
                    }
                }

                if (currentShowRight != _shouldShowRightHandsAndRays)
                {
                    if (_rightHand.TryGetComponent(out XRBaseController xrController) && xrController.model != null &&
                        xrController.model.TryGetComponent(out IHideable hideable))
                    {
                        if (_shouldShowRightHandsAndRays)
                            hideable.Show();
                        else
                            hideable.Hide();
                    }
                }
            }

        }
    }
}
