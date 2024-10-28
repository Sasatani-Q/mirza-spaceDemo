// /******************************************************************************
//  * File: XRPokeFollowTransform.cs
//  * Copyright (c) 2023 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
//  *
//  *
//  ******************************************************************************/

using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.State;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine;

namespace QCHT.Samples.XRKeyboard
{
    public class XRPokeFollowTransform : MonoBehaviour
    {
        [SerializeField] private Transform _followTransform;

        [SerializeField] private float _smoothSpeed = 9f;

        [SerializeField] private float _maxDistance = 20f;

        private Vector3 _initPos;
        private Vector3 _targetPos;

        private IPokeStateDataProvider _pokeStateDataProvider;

        protected void Awake()
        {
            FindPokeProvider();

            if (_pokeStateDataProvider == null)
            {
                Debug.Log("[XRPokeFollowTransform:Awake] Can't find poke provider in parent.");
                enabled = false;
            }
        }

        protected void Start()
        {
            if (_followTransform == null)
                return;

            _initPos = _followTransform.localPosition;
        }

        protected void OnEnable() => _pokeStateDataProvider?.pokeStateData?.SubscribeAndUpdate(OnPokeDataUpdated);

        protected void OnDisable() => _pokeStateDataProvider?.pokeStateData?.Unsubscribe(OnPokeDataUpdated);

        private void LateUpdate()
        {
            var dt = Time.deltaTime * _smoothSpeed;
            _followTransform.localPosition = Vector3.Lerp(_followTransform.localPosition, _targetPos, dt);
        }

        private void OnPokeDataUpdated(PokeStateData data)
        {
            var pokeTransform = data.target;
            var hasToFollowPoke = pokeTransform != null && pokeTransform.IsChildOf(transform);

            if (hasToFollowPoke)
            {
                var position = pokeTransform.InverseTransformPoint(data.axisAlignedPokeInteractionPoint);
                var maxDistanceReached = position.sqrMagnitude > Mathf.Sqrt(_maxDistance);
                if (maxDistanceReached)
                    position = Vector3.ClampMagnitude(position, _maxDistance);

                _targetPos = position;
            }
            else
                _targetPos = _initPos;
        }
        
        private void FindPokeProvider() => _pokeStateDataProvider ??= GetComponentInParent<IPokeStateDataProvider>();
    }
}