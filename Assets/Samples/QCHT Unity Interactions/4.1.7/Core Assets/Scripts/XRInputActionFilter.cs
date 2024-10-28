// /******************************************************************************
//  * File: XRInputActionFilter.cs
//  * Copyright (c) 2023 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
//  *
//  *
//  ******************************************************************************/

using UnityEngine;
using UnityEngine.InputSystem;

namespace QCHT.Interactions.Distal
{
    public class XRInputActionFilter : MonoBehaviour, IXRRayInteractorFilter
    {
        [SerializeField] protected InputActionProperty _inputAction;

        public virtual bool CanShowRay => _inputAction.action?.IsInProgress() ?? true;
    }
}