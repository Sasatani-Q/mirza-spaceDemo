// /******************************************************************************
//  * File: XRSnapUIActivator.cs
//  * Copyright (c) 2024 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
//  *
//  *
//  ******************************************************************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace QCHT.Interactions.UI
{
    public class XRSnapUIActivator : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;

        private readonly List<XRBaseInteractable> _interactables = new List<XRBaseInteractable>();
        private readonly List<XRInteractableSnapVolume> _snapVolumes = new List<XRInteractableSnapVolume>();
        private readonly List<Collider> _colliders = new List<Collider>();

        private bool _hasCanvas;
        private bool _hasBackCollider;

        public UnityEvent<bool> onSnapChanged = new UnityEvent<bool>();
        
        private void Awake()
        {
            if (canvas == null)
            {
                canvas = GetComponentInChildren<Canvas>();
            }

            _hasCanvas = canvas != null;

            _interactables.AddRange(GetComponentsInChildren<XRBaseInteractable>(true));
            _snapVolumes.AddRange(GetComponentsInChildren<XRInteractableSnapVolume>(true));
            _colliders.AddRange(GetComponentsInChildren<Collider>(true));
        }

        private void OnEnable() => SetEnabled(true);

        private void OnDisable() => SetEnabled(false);

        private void SetEnabled(bool enable)
        {
            if (_hasCanvas)
            {
                if (canvas.TryGetComponent<GraphicRaycaster>(out var graphicRaycaster))
                {
                    graphicRaycaster.enabled = !enable;
                }

                if (canvas.TryGetComponent<TrackedDeviceGraphicRaycaster>(out var trackedDeviceGraphicRaycaster))
                {
                    trackedDeviceGraphicRaycaster.enabled = !enable;
                }
            }

            foreach (var interactable in _interactables)
            {
                interactable.enabled = enable;
            }

            foreach (var snapVolume in _snapVolumes)
            {
                snapVolume.enabled = enable;
            }

            foreach (var col in _colliders)
            {
                col.enabled = enable;
            }
            
            onSnapChanged?.Invoke(enable);
        }
    }
}