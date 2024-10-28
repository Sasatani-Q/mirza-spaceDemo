// /******************************************************************************
//  * File: KeyButton.cs
//  * Copyright (c) 2023 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
//  *
//  *
//  ******************************************************************************/

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace QCHT.Samples.XRKeyboard
{
    [System.Serializable]
    public class KeyEvent : UnityEvent<KeyButton>
    {

    }

    public abstract class KeyButton : MonoBehaviour, IPointerExitHandler, IPointerDownHandler,
        IPointerUpHandler
    {
        [FormerlySerializedAs("_selectButton")] [SerializeField] private GameObject _selectState;
        [SerializeField] private AudioSource _audioSource;
        public KeyEvent inputEvent;
        
        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (_selectState) _selectState.SetActive(false);
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (_selectState) _selectState.SetActive(true);
            if (_audioSource) _audioSource.Play();
            inputEvent?.Invoke(this);
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            if (_selectState) _selectState.SetActive(false);
        }
    }

    public enum KeySpecial
    {
        None,
        Delete,
        Shift,
        Enter,
        DeleteAll,
        SwitchObject
    }
}
