﻿using UnityEngine;
using CloudsOfAvarice.StateEnumerators;

namespace CloudsOfAvarice
{
    public class PlaceUIOnObjectWhenTargeting : MonoBehaviour
    {
        #region Variables
        [Header("Targeting radius for targets and general settings")]
        public float radiusToCheck;                     // The radius to check for targets
        public string tagToTarget;                      // The tag on which to search for targets
        public LayerMask layerToCheck;                  // The layer in to check for targets
        public float distanceToRemoveTargeting;         // If object is too far away, remove targeting

        [Header("Automatic re-target on death")]
        public bool retargetOnTargetDeath;

        [Header("Setup for canvas, UI element to display and camera")]
        public Canvas completeViewportCanvas;           // This needs to be the complete screen.
        public GameObject objectToDisplay;              // Which gameobject to display when targeting.
        public Camera cameraToShowTargetFrom;           // Which camera to get the viewport from. 

        //Internal variables
        RectTransform completeScreenRectTransform;      // The entire screen viewport.
        Transform nearestEntityToTarget;                // The transform position of the nearest target.

        bool currentlyTargeting;                        // Bool for keeping track of targeting is active.

        // Which player to track.
        GameObject localEntity;
        IMovement movementScript;                      
        #endregion

        #region Inital setup
        // Use this for initialization
        void Start()
        {
            objectToDisplay.SetActive(false);
            currentlyTargeting = false;
            nearestEntityToTarget = null;

            completeScreenRectTransform = completeViewportCanvas.GetComponent<RectTransform>();

            if (tagToTarget == "")
                tagToTarget = Tags.ENEMY;

            //If player movement will be decouple, remove this. Also decouple input referenses in fixed update
            FindAndSetLocalPlayer();
            movementScript = localEntity.GetComponent<IMovement>();
        }

        void FindAndSetLocalPlayer()
        {
            localEntity = GameObject.FindGameObjectWithTag(Tags.PLAYER);
        }
        #endregion

        #region Runetime placement
        void FixedUpdate()
        {
            if (currentlyTargeting)
            {
                PlaceAndMaintainUIOnTarget();
                if (Vector3.Distance(nearestEntityToTarget.position, localEntity.transform.position) > distanceToRemoveTargeting || Input.GetButtonDown(Inputs.TARGET))
                    RemoveTargeting();
                if (retargetOnTargetDeath /*&& and check for target health*/)
                    FindAndTargetClosestObjectByLayerAndTag();
            }
            else
            {
                if (Input.GetButtonDown(Inputs.TARGET))
                    FindAndTargetClosestObjectByLayerAndTag();
            }
        }

        void PlaceAndMaintainUIOnTarget()
        {
            if (nearestEntityToTarget != null)
                objectToDisplay.transform.position = WorldToCanvasPosition(nearestEntityToTarget.position);
        }
        #endregion

        #region Visual management
        void FindAndTargetClosestObjectByLayerAndTag()
        {
            //Get nearbyPotentialTargets in radius by layer
            Collider[] nearbyPotentialTargets = Physics.OverlapSphere(localEntity.transform.position, radiusToCheck, layerToCheck);

            // Set max distance
            float closestDistanceSqr = radiusToCheck;

            // Set the current position
            Vector3 currentPosition = localEntity.transform.position;

            foreach (Collider potentialTarget in nearbyPotentialTargets)
            {
                if (potentialTarget.tag == Tags.ENEMY)
                {
                    Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;

                    // Square the distance
                    float dSqrToTarget = directionToTarget.sqrMagnitude;

                    // If the distance is shorter than the max check, set the target and new max.
                    if (dSqrToTarget < closestDistanceSqr)
                    {
                        closestDistanceSqr = dSqrToTarget;
                        nearestEntityToTarget = potentialTarget.transform;
                    }
                }
            }

            // If a target is found, set targeting as true and move state to targeting
            if (nearestEntityToTarget != null)
                ApplyTargeting();
        }

        void ApplyTargeting()
        {
            currentlyTargeting = true;
            PlaceAndMaintainUIOnTarget();
            ToggleTargetVisual();
        }

        void RemoveTargeting()
        {
            currentlyTargeting = false;
            ToggleTargetVisual();
        }

        void ToggleTargetVisual()
        {
            // Clear nearest entity if not targeting is active
            if(!currentlyTargeting)
                nearestEntityToTarget = null;

            // Call movement script to set lookAt() true/false and target 
            movementScript.ToggleTargeting(currentlyTargeting, nearestEntityToTarget);

            // Set targeting object true/false
            objectToDisplay.SetActive(currentlyTargeting);
        }

        //Translate the target position from worldspace to canvas space
        Vector3 WorldToCanvasPosition(Vector3 positionOfTarget)
        {
            //Get worldspace(vector3) from camera and translate to canvas (vector2)
            Vector2 positionOnCanvasFromWorldSpace = cameraToShowTargetFrom.WorldToViewportPoint(positionOfTarget);

            //Calculate position considering our percentage, using our canvas size
            //So if canvas size is (1100,500), and percentage is (0.5,0.5), current value will be (550,250)
            positionOnCanvasFromWorldSpace.x *= completeScreenRectTransform.sizeDelta.x;
            positionOnCanvasFromWorldSpace.y *= completeScreenRectTransform.sizeDelta.y;

            return positionOnCanvasFromWorldSpace;
        }
        #endregion
    }
}