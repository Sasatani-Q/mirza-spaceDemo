// /******************************************************************************
//  * File: XRFlipRatioFilter.cs
//  * Copyright (c) 2023 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
//  *
//  *
//  ******************************************************************************/

namespace QCHT.Interactions.Distal
{
    public class XRFlipRatioFilter : XRInputActionFilter
    {
        public override bool CanShowRay => (_inputAction.action?.ReadValue<float>() ?? 0f) <= 0f;
    }
}