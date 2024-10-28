// /******************************************************************************
//  * File: KeyStringCharButton.cs
//  * Copyright (c) 2023 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
//  *
//  *
//  ******************************************************************************/

using UnityEngine;
using UnityEngine.UI;

namespace QCHT.Samples.XRKeyboard
{
    public class KeyStringCharButton : KeyStringButton
    {
        [SerializeField] private Text _character;

        public void SetMaj(bool maj) => _character.text = maj ? _character.text.ToUpper() : _character.text.ToLower();
    }
}