// /******************************************************************************
//  * File: FollowOrigin.cs
//  * Copyright (c) 2023 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
//  *
//  *
//  ******************************************************************************/

using QCHT.Interactions.Core;
using UnityEngine;

namespace QCHT.Samples.Drawing
{
    public class FollowOrigin : MonoBehaviour
    {
        private Transform _followTransform;

        private void Awake()
        {
            var cameraFloorOffsetObject = XROriginUtility.GetCameraFloorOffsetObject();
            _followTransform = cameraFloorOffsetObject
                ? cameraFloorOffsetObject.transform
                : XROriginUtility.GetOriginTransform();

            if (!_followTransform)
                DestroyImmediate(this);
        }

        private void Update()
        {
            if (!_followTransform) return;
            var t = transform;
            var originTransform = _followTransform.transform;
            t.position = originTransform.position;
            t.rotation = originTransform.rotation;
        }
    }
}