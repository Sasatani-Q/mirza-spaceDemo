using UnityEngine;
using Qualcomm.Snapdragon.Spaces.Samples;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace QONOQ.Snapdragon.Spaces.Samples
{
    public class ObjectPlacementController : SampleController
    {
        public InputActionReference TouchpadInputAction;
        public InputActionReference TriggerAction;
        public GameObject GizmoSurface;
        private bool _canPlaceAnchorGizmos = true;
        private GameObject _indicatorGizmo;
        private GameObject _transparentGizmo;
        private GameObject _surfaceGizmo;
        [SerializeField]
        private GameObject[] placeObject;
        private int placeObjectNumber = 0;
        [SerializeField]
        private GameObject placementObjectExample;
        private GameObject placementObjectExamplePrevious;
        private GameObject instantiatedObject;
        private bool _placeAnchorAtRaycastHit;
        private ARRaycastManager _raycastManager;

        private void Awake()
        {
            _raycastManager = FindObjectOfType<ARRaycastManager>();
        }

        public override void Start()
        {
            TriggerAction.action.performed += OnTriggerAction;
            TriggerAction.action.canceled += OnTriggerCanceled;
            _indicatorGizmo = new GameObject("IndicatorGizmo");
            _surfaceGizmo = Instantiate(GizmoSurface, _indicatorGizmo.transform.position, Quaternion.identity, _indicatorGizmo.transform);

            SelectPlaceObject(2);
        }

        private void Update()
        {
            Raycast();
            if (instantiatedObject)
            {
                instantiatedObject.transform.position = _indicatorGizmo.transform.position;
                var rotY = TouchpadInputAction.action.ReadValue<Vector2>().x * 180;
                instantiatedObject.transform.rotation = Quaternion.Euler(new Vector3(0, rotY, 0));
                var size = TouchpadInputAction.action.ReadValue<Vector2>().y + 1;
                instantiatedObject.transform.localScale = Vector3.one * size;
            }

            placementObjectExample.transform.Rotate(0f, 40.0f * Time.deltaTime, 0f);
        }

        private void Raycast()
        {
            _placeAnchorAtRaycastHit = false;
            Ray ray = new Ray(_arCamera.position, _arCamera.forward);
            List<ARRaycastHit> hitResults = new List<ARRaycastHit>();
            if (_raycastManager.Raycast(ray, hitResults))
            {
                _placeAnchorAtRaycastHit = true;
            }

            if (_placeAnchorAtRaycastHit)
            {
                if (!_surfaceGizmo.activeSelf)
                {
                    _surfaceGizmo.SetActive(true);
                    _transparentGizmo.SetActive(false);
                }

                _indicatorGizmo.transform.position = hitResults[0].pose.position;
                return;
            }
        }

        private void OnTriggerAction(InputAction.CallbackContext context)
        {
            PlaceObject();
        }

        private void OnTriggerCanceled(InputAction.CallbackContext context)
        {
            instantiatedObject = null;
        }

        public void OnPointerEnterEvent()
        {
            _canPlaceAnchorGizmos = false;
        }

        public void OnPointerExitEvent()
        {
            _canPlaceAnchorGizmos = true;
        }

        private void PlaceObject()
        {
            if (!_canPlaceAnchorGizmos)
            {
                return;
            }

            var placePosition = Vector3.zero;
            var targetPosition = _indicatorGizmo.transform.position;

            Ray ray = new Ray(transform.position, transform.up);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                placePosition = hit.point;
            }

            instantiatedObject = _placeAnchorAtRaycastHit ? Instantiate(placeObject[placeObjectNumber], targetPosition, Quaternion.identity) : Instantiate(placeObject[placeObjectNumber], targetPosition, Quaternion.identity);
        }

        public void SelectPlaceObject(int num)
        {
            placeObjectNumber = num;

            if (placementObjectExamplePrevious)
            {
                Destroy(placementObjectExamplePrevious);
            }
            placementObjectExamplePrevious = Instantiate(placeObject[placeObjectNumber], placementObjectExample.transform);
            placementObjectExamplePrevious.transform.localPosition = Vector3.zero;
        }
    }
}