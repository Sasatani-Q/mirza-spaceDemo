// /******************************************************************************
//  * File: CubeAppearanceFeedback.cs
//  * Copyright (c) 2023 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
//  *
//  *
//  ******************************************************************************/

using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;

namespace QCHT.Samples.Proximal
{
    public class CubeAppearanceFeedback : MonoBehaviour
    {
        [SerializeField] private Texture2D normalTexture;
        [SerializeField] private Texture2D hoverTexture;
        [SerializeField] private Texture2D selectedTexture;
        [SerializeField] private CubeLabel label;

        private XRGrabInteractable _interactable;

        public void OnEnable()
        {
            _interactable = GetComponentInParent<XRGrabInteractable>();

            if (_interactable)
            {
                _interactable.hoverEntered.AddListener(OnHoverEntered);
                _interactable.hoverExited.AddListener(OnHoverExited);
                _interactable.selectEntered.AddListener(OnSelectEntered);
                _interactable.selectExited.AddListener(OnSelectExited);
            }
        }

        public void OnDisable()
        {
            if (_interactable)
            {
                _interactable.hoverEntered.RemoveListener(OnHoverEntered);
                _interactable.hoverExited.RemoveListener(OnHoverExited);
                _interactable.selectEntered.RemoveListener(OnSelectEntered);
                _interactable.selectExited.RemoveListener(OnSelectExited);
            }
        }

        private void OnHoverEntered(HoverEnterEventArgs _) => UpdateAppearance();
        private void OnHoverExited(HoverExitEventArgs _) => UpdateAppearance();
        private void OnSelectEntered(SelectEnterEventArgs _) => UpdateAppearance();
        private void OnSelectExited(SelectExitEventArgs _) => UpdateAppearance();

        private void UpdateAppearance()
        {
            var material = GetComponent<Renderer>().material;

            if (label)
            {
                label.ShowLabel(!_interactable.isSelected);
            }

            if (_interactable.isSelected)
            {
                material.mainTexture = selectedTexture;
                return;
            }

            if (_interactable.isHovered)
            {
                material.mainTexture = hoverTexture;
                return;
            }

            material.mainTexture = normalTexture;
        }
    }
}