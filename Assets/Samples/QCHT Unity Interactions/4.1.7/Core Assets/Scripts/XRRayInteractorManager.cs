// /******************************************************************************
//  * File: XRRayInteractorManager.cs
//  * Copyright (c) 2023 Qualcomm Technologies, Inc. and/or its subsidiaries. All rights reserved.
//  *
//  *
//  ******************************************************************************/

using System.Collections.Generic;
using QCHT.Interactions.Core;
using QCHT.Interactions.Hands;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Object = UnityEngine.Object;

namespace QCHT.Interactions.Distal
{
    [DefaultExecutionOrder(RayInteractorManager)]
    public class XRRayInteractorManager : MonoBehaviour
    {
        // Before all interactions
        public const int RayInteractorManager = XRInteractionUpdateOrder.k_Interactors - 1;
        public const int BeforeRayInteractorManager = RayInteractorManager - 2;

        [SerializeField] protected XRRayInteractor rayInteractor;

        [Interface(typeof(IXRRayInteractorFilter))]
        [SerializeField] protected List<Object> startFilters = new List<Object>();

        private readonly List<IXRRayInteractorFilter> _filters = new List<IXRRayInteractorFilter>();

        private static readonly List<XRRayInteractorManager> s_activeViewers = new List<XRRayInteractorManager>();
        
        protected XrHandedness _handedness;

        protected void Awake()
        {
            _handedness = GetComponentInParent<IHandedness>()?.Handedness ?? XrHandedness.XR_HAND_LEFT;

            foreach (var filter in startFilters)
            {
                if (filter == null || !(filter is IXRRayInteractorFilter visualFilter))
                    return;

                _filters.Add(visualFilter);
            }

            s_activeViewers.Add(this);
        }

        protected void OnDestroy()
        {
            s_activeViewers.Remove(this);
        }

        public static void AddFilterToViewer(IXRRayInteractorFilter filter, XrHandedness handedness)
        {
            foreach (var viewer in s_activeViewers)
            {
                if (viewer._handedness == handedness)
                {
                    viewer.AddFilter(filter);
                }
            }
        }

        public static void RemoveFilterToViewer(IXRRayInteractorFilter filter)
        {
            foreach (var viewer in s_activeViewers)
            {
                viewer.RemoveFilter(filter);
            }
        }

        public void AddFilter(IXRRayInteractorFilter filter)
        {
            _filters.Add(filter);
        }

        public void RemoveFilter(IXRRayInteractorFilter filter)
        {
            _filters.Remove(filter);
        }

        protected virtual void Update()
        {
            if (rayInteractor != null)
            {
                var show = true;
                for (var i = 0; i < _filters.Count; i++)
                {
                    if (!_filters[i].CanShowRay)
                    {
                        show = false;
                    }
                }

                rayInteractor.enabled = show;
                // rayInteractor.enabled = _filters.Aggregate(true, (current, filter) => current & filter.CanShowRay);
            }
        }
    }
}