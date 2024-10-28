// /******************************************************************************
//  * File: KeyStringButton.cs
//  * Copyright (c) 2023 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
//  *
//  *
//  ******************************************************************************/

using UnityEngine;

namespace QCHT.Samples.XRKeyboard
{
    public class KeyStringButton : KeyButton
    {
        [SerializeField] private string _strInput = string.Empty;
        public string StrInput => _strInput;
    }
}