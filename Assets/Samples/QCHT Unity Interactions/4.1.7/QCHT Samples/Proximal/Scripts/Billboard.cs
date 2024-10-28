// /******************************************************************************
//  * File: Billboard.cs
//  * Copyright (c) 2023 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
//  *
//  *
//  ******************************************************************************/

using UnityEngine;

namespace QCHT.Samples.Proximal
{
    public class Billboard : MonoBehaviour
    {
        public Transform Target;

        public bool LockY;

        #region MonoBehaviour Functions

        private void LateUpdate()
        {
            Target = Target ? Target : Camera.main.transform;

            if (ReferenceEquals(Target, null))
                return;

            if (LockY)
            {
                var upVector = Vector3.up;
                var wPos = Vector3.Normalize(transform.position - Target.transform.position);
                var euler = Quaternion.LookRotation(wPos, upVector).eulerAngles;
                var rot = Quaternion.Euler(0, euler.y, 0);
                transform.rotation = rot;
            }
            else
            {
                var camRot = Target.transform.rotation;
                var upVector = camRot * Vector3.up;
                var wPos = transform.position + camRot * Vector3.forward;
                transform.LookAt(wPos, upVector);
            }
        }

        #endregion
    }
}