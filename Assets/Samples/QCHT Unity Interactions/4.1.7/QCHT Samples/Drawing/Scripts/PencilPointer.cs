// /******************************************************************************
//  * File: PencilPointer.cs
//  * Copyright (c) 2023 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
//  *
//  *
//  ******************************************************************************/

using System;
using System.Collections.Generic;
using QCHT.Interactions;
using QCHT.Interactions.Distal;
using QCHT.Interactions.Core;
using QCHT.Interactions.Extensions;
using QCHT.Interactions.Hands;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace QCHT.Samples.Drawing
{
    public class PencilPointer : MonoBehaviour, IXRRayInteractorFilter
    {
        [SerializeField] private XrHandedness handedness;
        [SerializeField] private InputActionProperty penSelectAction;

        [Space] [SerializeField, Tooltip("Should it re-calculate pen down/up positions using hat subsystem?")]
        private bool useSubsystem;

        [SerializeField,
         Tooltip(
             "When using hat subsystem, it corresponds to the thumb-to-index tips distance threshold where the pencil will be considered down?")]
        private float penDownValue;

        [SerializeField,
         Tooltip(
             "When using hat subsystem, it corresponds to the thumb-to-index tips distance threshold where the pencil will be considered up?")]
        private float penUpValue;

        [Space] [SerializeField] private ParticleSystem pointerParticles;
        [SerializeField] private ParticleSystem lineParticles;
        [SerializeField] private float minSizeMultiplier = 1f;

        public event Action<XrHandedness> onPenDown;
        public event Action<XrHandedness> onPenUp;

        public bool IsDrawing { get; private set; }
        public bool CanShowRay => !IsDrawing;

        private XRHandTrackingSubsystem _subsystem;
        private XRSwitchHandToControllerManager _controllerManager;

        private XRInteractionManager _interactionManager;
        private readonly List<XRRayInteractor> _rayInteractors = new List<XRRayInteractor>();

        private bool _firstFrame;
        private float _referenceScale = 1f;

        private void Awake()
        {
            _controllerManager = FindObjectOfType<XRSwitchHandToControllerManager>();
            _interactionManager = FindObjectOfType<XRInteractionManager>();

            // Get all ray interactors for this hand
            var interactors = new List<IXRInteractor>();
            _interactionManager.GetRegisteredInteractors(interactors);

            foreach (var interactor in interactors)
            {
                var rayInteractor = interactor as XRRayInteractor;
                if (rayInteractor == null)
                    continue;

                var hand = rayInteractor.GetComponentInParent<XRHandedness>();
                if (hand == null || hand.Handedness != handedness)
                    continue;

                _rayInteractors.Add(rayInteractor);
            }
        }

        private void OnEnable()
        {
            penSelectAction.EnableDirectAction();
            XRRayInteractorManager.AddFilterToViewer(this, handedness);

            _firstFrame = true;
        }

        private void OnDisable()
        {
            penSelectAction.DisableDirectAction();
            XRRayInteractorManager.RemoveFilterToViewer(this);
        }

        private void Update()
        {
            if (_firstFrame)
            {
                _firstFrame = false;
                return;
            }

            if (useSubsystem)
            {
                _subsystem ??= XRHandTrackingSubsystem.GetSubsystemInManager();
            }

            if (!IsDrawing && WasPressedThisFrame())
            {
                IsDrawing = true;
                StartLineParticles();
                onPenDown?.Invoke(handedness);
            }
            else if (IsDrawing && WasReleasedThisFrame())
            {
                IsDrawing = false;
                StopLineParticles();
                onPenUp?.Invoke(handedness);
            }
        }

        private bool WasPressedThisFrame()
        {
            foreach (var xrRay in _rayInteractors)
            {
                if (xrRay.IsOverUIGameObject() || xrRay.hasHover)
                {
                    return false;
                }
            }

            if (useSubsystem && _subsystem != null)
            {
                if (_controllerManager != null && _controllerManager.CurrentMode ==
                    XRSwitchHandToControllerManager.ControllerMode.HandTracking)
                {
                    var hand = handedness == XrHandedness.XR_HAND_LEFT ? _subsystem.LeftHand : _subsystem.RightHand;
                    var pinchStrength = hand.GetFingerPinching(XrFinger.XR_HAND_FINGER_INDEX);
                    return pinchStrength > penDownValue;
                }
            }

            // Otherwise check penSelectAction value
            if (penSelectAction.action != null && penSelectAction.action.controls.Count > 0)
            {
                return penSelectAction.action.WasPressedThisFrame();
            }

            return false;
        }

        private bool WasReleasedThisFrame()
        {
            foreach (var xrRay in _rayInteractors)
            {
                if (xrRay.IsOverUIGameObject() || xrRay.hasHover)
                {
                    return true;
                }
            }

            if (useSubsystem && _subsystem != null)
            {
                if (_controllerManager != null && _controllerManager.CurrentMode ==
                    XRSwitchHandToControllerManager.ControllerMode.HandTracking)
                {
                    var hand = handedness == XrHandedness.XR_HAND_LEFT ? _subsystem.LeftHand : _subsystem.RightHand;
                    var pinchStrength = hand.GetFingerPinching(XrFinger.XR_HAND_FINGER_INDEX);
                    return pinchStrength < penUpValue;
                }
            }

            // Otherwise check penSelectAction value
            if (penSelectAction.action != null && penSelectAction.action.controls.Count > 0)
            {
                return penSelectAction.action.WasReleasedThisFrame();
            }

            return true;
        }

        public void SetColor(Color color)
        {
            var main = pointerParticles.main;
            main.startColor = color;
        }

        public void SetScale(float scale)
        {
            _referenceScale = scale;
        }

        public void SetLineParticles(ParticleSystem particles)
        {
            lineParticles = particles;
            var particleSystemTransform = particles.transform;
            particleSystemTransform.SetParent(transform);
            particleSystemTransform.localPosition = Vector3.zero;
        }

        public void DestroyLineParticles()
        {
            if (lineParticles == null)
                return;

            Destroy(lineParticles.gameObject);
            lineParticles = null;
        }

        public void StartLineParticles()
        {
            if (lineParticles != null)
            {
                lineParticles.Play();
            }
        }

        public void StopLineParticles()
        {
            if (lineParticles != null)
            {
                lineParticles.Stop();
            }
        }

        public void UpdateScale()
        {
            if (pointerParticles == null)
            {
                return;
            }

            var main = pointerParticles.main;
            main.startSize = minSizeMultiplier * _referenceScale;
        }

        public void Show()
        {
            if (pointerParticles != null)
            {
                pointerParticles.Play();
            }
        }

        public void Hide()
        {
            if (pointerParticles != null)
            {
                pointerParticles.Stop();
            }
        }
    }
}