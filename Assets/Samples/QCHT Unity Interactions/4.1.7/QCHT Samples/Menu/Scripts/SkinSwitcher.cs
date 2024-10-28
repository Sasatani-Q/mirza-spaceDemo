// /******************************************************************************
//  * File: SkinSwitcher.cs
//  * Copyright (c) 2023 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
//  *
//  *
//  ******************************************************************************/

using QCHT.Interactions.Core;
using QCHT.Interactions.Hands;
using UnityEngine;

namespace QCHT.Samples.Menu
{
    public class SkinSwitcher : MonoBehaviour
    {
        [SerializeField] private XRHandTrackingManager _hatManager;

        public void SetLeftSkin(HandSkin skin)
        {
            FindXRHandTrackingManager();

            if (_hatManager != null)
                _hatManager.LeftHandSkin = skin;
        }

        public void SetRightSkin(HandSkin skin)
        {
            FindXRHandTrackingManager();

            if (_hatManager != null)
                _hatManager.RightHandSkin = skin;
        }

        public void SetLeftHandPrefab(GameObject prefab)
        {
            FindXRHandTrackingManager();

            if (_hatManager != null)
            {
                _hatManager.LeftHandPrefab = prefab;
                _hatManager.RefreshLeftHand();
            }
        }

        public void SetRightHandPrefab(GameObject prefab)
        {
            FindXRHandTrackingManager();

            if (_hatManager != null)
            {
                _hatManager.RightHandPrefab = prefab;
                _hatManager.RefreshRightHand();
            }
        }

        private void FindXRHandTrackingManager() =>
            _hatManager = _hatManager != null ? _hatManager : FindObjectOfType<XRHandTrackingManager>(true);
    }
}