// /******************************************************************************
//  * File: ResetOriginToCameraOnLoad.cs
//  * Copyright (c) 2023 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
//  *
//  *
//  ******************************************************************************/

#if UNITY_AR_FOUNDATION_LEGACY
using UnityEngine.XR.ARFoundation;
#endif

using QCHT.Interactions.Core;
using UnityEngine;

namespace QCHT.Samples.Menu
{
    public class ResetOriginToCameraOnLoad : MonoBehaviour
    {
        public bool ResetSessionOriginOnStart = true;

        private bool _isSessionOriginMoved;

        private void OnEnable() => OffsetSessionOrigin();

        private void Update()
        {
            var cameraInOriginSpaces = Vector3.zero;
            var isSet = false;

            if (XROriginUtility.FindXROrigin() is var xrOrigin && xrOrigin != null)
            {
                cameraInOriginSpaces = xrOrigin.CameraInOriginSpacePos;
                isSet = true;
            }
            
#if UNITY_AR_FOUNDATION_LEGACY
            if (!isSet && XROriginUtility.FindARSessionOrigin() is var arSessionOrigin && arSessionOrigin != null)
            {
                cameraInOriginSpaces = arSessionOrigin.transform.InverseTransformPoint(arSessionOrigin.camera.transform.position);
                isSet = true;
            }
#endif
            
            if (ResetSessionOriginOnStart && !_isSessionOriginMoved && cameraInOriginSpaces != Vector3.zero && isSet)
            {
                OffsetSessionOrigin();
                _isSessionOriginMoved = true;
            }
        }

        public void Recenter()
        {
            _isSessionOriginMoved = false;
        }

        private void OffsetSessionOrigin()
        {
            Transform sessionOrigin = null;
            Transform cameraTransform = null;
            var isSet = false;
            
            if (XROriginUtility.FindXROrigin() is var xrOrigin && xrOrigin != null)
            {
                sessionOrigin = xrOrigin.Origin.transform;
                cameraTransform = xrOrigin.Camera.transform;
                isSet = true;
            }
            
#if UNITY_AR_FOUNDATION_LEGACY
            if (!isSet && XROriginUtility.FindARSessionOrigin() is var arSessionOrigin && arSessionOrigin != null)
            {
                sessionOrigin = arSessionOrigin.transform;
                cameraTransform = arSessionOrigin.camera.transform;
            }
#endif
            
            if (sessionOrigin != null && cameraTransform != null)
            {
                var t = sessionOrigin.transform;
                t.Rotate(0.0f, -cameraTransform.rotation.eulerAngles.y, 0.0f, Space.World);
                t.position -= cameraTransform.position;
            }
        }
    }
}