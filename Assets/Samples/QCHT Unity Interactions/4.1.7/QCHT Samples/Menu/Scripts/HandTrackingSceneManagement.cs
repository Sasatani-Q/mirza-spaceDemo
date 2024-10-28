// /******************************************************************************
//  * File: HandTrackingSceneManagement.cs
//  * Copyright (c) 2024 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
//  *
//  *
//  ******************************************************************************/

using QCHT.Interactions.Core;
using UnityEngine;

namespace QCHT.Samples.Menu
{
    public class HandTrackingSceneManagement : MonoBehaviour
    {
        [SerializeField, Tooltip("Should stop hand tracking OnDestroy?")] 
        private bool shouldStopOnDisable;
        
        private XRHandTrackingSubsystem _handTrackingSubsystem;
        
        public void OnEnable()
        {
            if (TryToFindHandTrackingSubsystem(out var subsystem))
            {
                subsystem.Start();
            }
        }

        public void OnDisable()
        {
            if (shouldStopOnDisable)
            {
                if (TryToFindHandTrackingSubsystem(out var subsystem))
                {
                    subsystem.Stop();
                }
            }
        }
        
        private bool TryToFindHandTrackingSubsystem(out XRHandTrackingSubsystem subsystem)
        {
            if (_handTrackingSubsystem != null)
            {
                subsystem = _handTrackingSubsystem;
                return true;
            }
            
            _handTrackingSubsystem = subsystem = XRHandTrackingSubsystem.GetSubsystemInManager();
            return _handTrackingSubsystem != null;
        }
    }
}