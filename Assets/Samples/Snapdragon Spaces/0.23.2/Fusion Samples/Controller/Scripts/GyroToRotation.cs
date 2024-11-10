/******************************************************************************
 * File: GyroToRotation.cs
 * Copyright (c) 2023-2024 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
 *
 ******************************************************************************/

using UnityEngine;

namespace Qualcomm.Snapdragon.Spaces.Samples
{
    public class GyroToRotation : MonoBehaviour
    {
        public Camera xrCamera;
        public GameObject controllerRepresentation;
        public Vector3 rotationRate = new Vector3(0, 0, 0);

        public Vector3 RotationRate
        {
            get => rotationRate;
            set => rotationRate = value;
        }

        private void Awake()
        {
            if (xrCamera == null)
            {
                xrCamera = OriginLocationUtility.GetOriginCamera();
            }

            EnableGyro(true);
        }

        private void Start()
        {
            ResetGyro();
        }

        protected void Update()
        {
            if (Input.gyro.enabled)
            {
                rotationRate = Input.gyro.rotationRate;
                float rx = -rotationRate.x;
                float ry = -rotationRate.z;
                float rz = -rotationRate.y;
                rotationRate.x = rx;
                rotationRate.y = ry;
                rotationRate.z = rz;
                controllerRepresentation.transform.Rotate(RotationRate);
            }
        }

        public void EnableGyro(bool isOn)
        {
            Input.gyro.enabled = isOn;
        }

        public void ResetGyro()
        {
            Vector3 forward = xrCamera.transform.forward;
            forward.y = 0;
            controllerRepresentation.transform.forward = forward; // rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
