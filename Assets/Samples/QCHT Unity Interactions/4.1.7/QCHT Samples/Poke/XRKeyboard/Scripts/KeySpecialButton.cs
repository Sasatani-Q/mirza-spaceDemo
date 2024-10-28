// /******************************************************************************
//  * File: KeySpecialButton.cs
//  * Copyright (c) 2023 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
//  *
//  *
//  ******************************************************************************/

using UnityEngine;

namespace QCHT.Samples.XRKeyboard 
{
    public class KeySpecialButton : KeyButton
    {
        [SerializeField] private KeySpecial keyKeySpecial;
        [SerializeField] private GameObject _objectToDeactivate, _objectToActivate;

        private void Start()
        {
            if (keyKeySpecial == KeySpecial.None)
                this.enabled = false;
        }

        public void SwitchObject()
        {
            Debug.Log("Switch object");
            if (!_objectToActivate || !_objectToDeactivate)
                return;
            _objectToActivate.SetActive(true);
            _objectToDeactivate.SetActive(false);
        }

        public KeySpecial KeySpecial => keyKeySpecial;
    }
}