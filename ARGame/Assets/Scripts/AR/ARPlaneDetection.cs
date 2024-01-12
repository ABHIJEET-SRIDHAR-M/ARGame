using System;
using System.Collections.Generic;
using Managers;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace AR
{
    /*
    [RequireComponent(typeof(ARPlaneManager))]
    */
    public class ARPlaneDetection : MonoBehaviour
    {

        public GameObject groundPrefab;

        public GameObject anchoredGround;
        private ARPlaneManager _arPlaneManager;
        private ARAnchorManager _anchorManager;
        private ScreenManager _screenManager;
        
        public TrackableId _planeID = TrackableId.invalidId;
        
        // Start is called before the first frame update
        void Start()
        {
            SetupARComponents();
            OffPlaneDetection();
        }

        private void SetupARComponents()
        {
            // Get the ARPlaneManager component attached to the AR Session Origin
            _arPlaneManager = FindObjectOfType<ARPlaneManager>();
            _screenManager = FindObjectOfType<ScreenManager>();
            _anchorManager = GetComponent<ARAnchorManager>();

            // Subscribe to plane change events
            if (_arPlaneManager != null)
            {
                _arPlaneManager.planesChanged += OnPlanesChanged;
            }

            
        }

        private void OnPlanesChanged(ARPlanesChangedEventArgs eventArgs)
        {
            // Iterate through added planes
            foreach (var addedPlane in eventArgs.added)
            {
                //HandlePlane("Added", addedPlane);
            }

            // Iterate through updated planes
            foreach (var updatedPlane in eventArgs.updated)
            {
                HandlePlane("Updated", updatedPlane);
            }

            // Iterate through removed planes
            foreach (var removedPlane in eventArgs.removed)
            {
                //HandlePlane("Removed", removedPlane);
            }
        }
        
        private void HandlePlane(string action, ARPlane plane)
        {
            if (_planeID == TrackableId.invalidId)
            {
                var planeArea = plane.extents.x * plane.extents.y;
                if (planeArea > 0.01f)
                {
                    _screenManager.ShowScreen(Enums.UIScreen.RotationMenu );
                    CreateAnchorAndPlaceGround(plane);
                    _planeID = plane.trackableId;
                    OffPlaneDetection();
                }
            }
            else
            {
                if (plane.trackableId == _planeID)
                {
                    var planeArea = plane.extents.x * plane.extents.y;
                    if (planeArea > 0.05f)
                    {
                        //OffPlaneDetection();
                    }
                }
            }
            
        }
        private void CreateAnchorAndPlaceGround(ARPlane plane)
        {
            var oldPrefab = _anchorManager.anchorPrefab;
            _anchorManager.anchorPrefab = groundPrefab;
            var anchor = _anchorManager.AttachAnchor(plane, new Pose(plane.center, Quaternion.identity));
            anchoredGround = anchor.gameObject;
            _anchorManager.anchorPrefab = oldPrefab;
        }
        

        private void OnDestroy()
        {
            // Unsubscribe from plane change events when the script is destroyed
            if (_arPlaneManager != null)
            {
                _arPlaneManager.planesChanged -= OnPlanesChanged;
            }
        }
        
        public void OffPlaneDetection()
        {
            if (_arPlaneManager)
            {
                _arPlaneManager.SetTrackablesActive(false);
                
                // Toggle plane detection state
                _arPlaneManager.enabled = false;
                
            }

            var points = FindObjectOfType<ARPointCloudManager>();
            if (points)
            {
                points.SetTrackablesActive(false);
                points.enabled = false;
            }
            
        }
        
        public void OnPlaneDetection()
        {
            if (_arPlaneManager)
            {
                _arPlaneManager.SetTrackablesActive(true);
                
                // Toggle plane detection state
                _arPlaneManager.enabled = true;
                
            }

            var points = FindObjectOfType<ARPointCloudManager>();
            if (points)
            {
                points.SetTrackablesActive(true);
                points.enabled = true;
            }
            
        }

        
    }
}
