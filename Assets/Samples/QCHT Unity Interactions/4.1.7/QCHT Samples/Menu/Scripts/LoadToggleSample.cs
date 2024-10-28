// /******************************************************************************
//  * File: LoadToggleSample.cs
//  * Copyright (c) 2023 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
//  *
//  *
//  ******************************************************************************/

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace QCHT.Samples.Menu
{
    [RequireComponent(typeof(Toggle))]
    public class LoadToggleSample : MonoBehaviour
    {
        [SerializeField] public SampleSettings _sampleToLoad;
        
        private Toggle _toggle;
        
        public UnityEvent<SampleSettings> OnSampleClicked = new UnityEvent<SampleSettings>();

        private void Awake() {
            _toggle = GetComponent<Toggle>();
        }

        private void OnEnable() {
            _toggle.onValueChanged.AddListener(OnToggleValueChanged);
        }

        private void OnDisable() {
            _toggle.onValueChanged.RemoveListener(OnToggleValueChanged);
        }

        public void OnToggleValueChanged(bool isOn) {
            if (isOn)
                OnSampleClicked?.Invoke(_sampleToLoad);
        }
    }
}