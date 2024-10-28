// /******************************************************************************
//  * File: IXRRayInteractorFilter.cs
//  * Copyright (c) 2023 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
//  *
//  *
//  ******************************************************************************/

namespace QCHT.Interactions.Distal
{
    public interface IXRRayInteractorFilter
    {
        public bool CanShowRay { get; }
    }
}