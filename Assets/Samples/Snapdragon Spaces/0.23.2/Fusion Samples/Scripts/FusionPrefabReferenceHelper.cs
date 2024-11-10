/******************************************************************************
 * File: FusionPrefabReferenceHelper.cs
 * Copyright (c) 2024 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
 *
 ******************************************************************************/
using UnityEngine;
using UnityEngine.InputSystem;

namespace Qualcomm.Snapdragon.Spaces.Samples
{
    public class FusionPrefabReferenceHelper: MonoBehaviour
    {
        public GameObject FusionPointer;
        public InputActionReference SwitchInputAction;
        public FusionLifecycleEvents FusionLifecycleEventsReference;
    }
}
