// /******************************************************************************
//  * File: CubeLabel.cs
//  * Copyright (c) 2023 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
//  *
//  *
//  ******************************************************************************/

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace QCHT.Samples.Proximal
{
    [DefaultExecutionOrder(XRInteractionUpdateOrder.k_BeforeRenderLineVisual)]
    public class CubeLabel : MonoBehaviour
    {
        [SerializeField] private GameObject label;
        [SerializeField] private Collider objectCollider;
        [SerializeField] private float labelYOffset = 0.01f;
        
        protected void OnEnable() => Application.onBeforeRender += OnBeforeRender;

        protected void OnDisable() => Application.onBeforeRender += OnBeforeRender;

        // protected void Update() => UpdateLabelPosition();

        [BeforeRenderOrder(XRInteractionUpdateOrder.k_BeforeRenderLineVisual)]
        protected void OnBeforeRender() => UpdateLabelPosition();
        
        protected void UpdateLabelPosition()
        {
            if (!label) return;
            var position = label.transform.position;
            if (objectCollider)
            {
                var bounds = objectCollider.bounds;
                var center = bounds.center;
                var size = bounds.size;
                position = center + new Vector3(0, size.y / 2, 0);
            }

            position.y += labelYOffset;
            label.transform.position = position;
        }
        
        public void ShowLabel(bool show)
        {
            if (label)
                label.SetActive(show);
        }
    }
}