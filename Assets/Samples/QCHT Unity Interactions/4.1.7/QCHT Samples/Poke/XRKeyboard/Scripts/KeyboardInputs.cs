// /******************************************************************************
//  * File: KeyboardInputs.cs
//  * Copyright (c) 2023 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
//  *
//  *
//  ******************************************************************************/

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace QCHT.Samples.XRKeyboard
{
    public class KeyboardInputs : MonoBehaviour
    {
        [SerializeField] private Text _keyboardInputsText;

        public Text KeyboardInputsText
        {
            get => _keyboardInputsText;
            set => _keyboardInputsText = value;
        }

        private bool _maj;

        public bool IsMaj => _maj;

        public UnityEvent<KeyButton> OnKeyButtonPressed = new UnityEvent<KeyButton>();

        private List<KeyButton> _keyButtons;

        private void Start()
        {
            _keyButtons = GetComponentsInChildren<KeyButton>(true).ToList();
            foreach (KeyButton button in _keyButtons)
            {
                button.inputEvent.AddListener(RegisterInput);
            }
        }

        private void RegisterInput(KeyButton keyButton)
        {
            ProcessText(keyButton);
            OnKeyButtonPressed?.Invoke(keyButton);
        }

        private void ProcessText(KeyButton keyButton)
        {
            if (_keyboardInputsText == null)
                return;
            
            var keyStringCharButton = keyButton as KeyStringCharButton;
            if (keyStringCharButton != null && keyStringCharButton.StrInput.Length > 0)
            {
                _keyboardInputsText.text += _maj ? keyStringCharButton.StrInput.ToUpper() : keyStringCharButton.StrInput.ToLower();
                return;
            }
             
            var keyStringButton = keyButton as KeyStringButton;
            if (keyStringButton != null && keyStringButton.StrInput.Length > 0)
            {
                _keyboardInputsText.text += keyStringButton.StrInput;
                return;
            }

            var keySpecialButton = keyButton as KeySpecialButton;
            if (keySpecialButton != null)
            {
                switch (keySpecialButton.KeySpecial)
                {
                    case KeySpecial.Delete:
                        if (_keyboardInputsText.text.Length > 3 && _keyboardInputsText.text.Substring(_keyboardInputsText.text.Length - 2, 2) == "\n") 
                            _keyboardInputsText.text = _keyboardInputsText.text.Substring(0, _keyboardInputsText.text.Length - 2);
                        else if (_keyboardInputsText.text.Length > 0)
                            _keyboardInputsText.text = _keyboardInputsText.text.Substring(0, _keyboardInputsText.text.Length - 1);
                        break;
                    case KeySpecial.Enter:
                        _keyboardInputsText.text += "\n";
                        break;
                    case KeySpecial.Shift:
                        SetMaj(!_maj);
                        break;
                    case KeySpecial.DeleteAll:
                        _keyboardInputsText.text = string.Empty;
                        break;
                    case KeySpecial.SwitchObject:
                        keySpecialButton.SwitchObject();
                        break;
                }
            }
        }

        private void SetMaj(bool maj)
        {
            foreach (var key in _keyButtons)
            {
                var keyChar = key as KeyStringCharButton;
                if (keyChar != null)
                    keyChar.SetMaj(maj);
            }
            _maj = maj;
        }
    }
}

