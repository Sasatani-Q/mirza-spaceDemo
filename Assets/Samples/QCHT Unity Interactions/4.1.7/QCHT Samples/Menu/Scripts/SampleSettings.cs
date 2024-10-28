// /******************************************************************************
//  * File: SampleSettings.cs
//  * Copyright (c) 2023 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
//  *
//  *
//  ******************************************************************************/

using UnityEngine;

namespace QCHT.Samples.Menu
{
    [CreateAssetMenu(fileName = "NewSampleSettings", menuName = "QCHT/Samples/SampleSettings", order = 0)]
    public class SampleSettings : ScriptableObject
    {
        public string SceneName;
        public bool HasVff;
        public bool EnablePhysicRaycast;
    }
}