﻿using CloudsOfAvarice.StateEnumerators;
using UnityEngine;

namespace CloudsOfAvarice
{
    public class ToggleTargetOnEnemy : MonoBehaviour
    {
        #region Variables
        [Header("Targeting radius and general settings")]
        public float radiusToCheck;                     // The radius to check for targets
        public string tagToTarget;                      // The tag on which to search for targets
        public LayerMask layerToCheck;                  // The layer in to check for targets
        public float distanceToRemoveTargeting;         // If object is too far away, remove targeting

        public GameObject targetCanvas;                 // Where to find script that controls target circle

        [Header("Automatic re-target on death")]
        public bool retargetOnTargetDeath;

        Transform nearestTarget;                        // The transform position of the nearest target
        float closestDistanceSqr;                       // The closest distance to the target
        Vector3 currentPosition;                        // The current position of this object
        bool currentlyTargeting;                        // Bool for keeping track of targeting is active
        IMovement movementScript;                       // Reference for the movement script to control turning
        TargetState stateOfTargeting;                   // The state of targeting
        #endregion

        void Start()
        {
            if (tagToTarget == "")
                tagToTarget = Tags.ENEMY;

            stateOfTargeting = TargetState.NoTarget;
            movementScript = GetComponent<IMovement>();

            currentlyTargeting = false;
            nearestTarget = null;
        }

        void Update()
        {
            switch (stateOfTargeting)
            {
                case TargetState.NoTarget:
                    if (Input.GetButtonDown(Inputs.TARGET))
                        FindAndTargetClosestObjectByLayerAndTag();
                    break;
                case TargetState.Targeting:
                    if (Vector3.Distance(nearestTarget.position, transform.position) > distanceToRemoveTargeting || Input.GetButtonDown(Inputs.TARGET))
                        RemoveTargeting();
                    if (retargetOnTargetDeath /*&& and check for target health*/)
                        FindAndTargetClosestObjectByLayerAndTag();
                    break;
            }
        }
        #region Visual manager
        void FindAndTargetClosestObjectByLayerAndTag()
        {
            //Get objects in radius by layer
            Collider[] objects = Physics.OverlapSphere(transform.position, radiusToCheck, layerToCheck);

            // Set max distance
            closestDistanceSqr = radiusToCheck;

            // Set the current position
            currentPosition = transform.position;

            foreach (Collider potentialTarget in objects)
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
                        nearestTarget = potentialTarget.transform;
                    }
                }
            }

            // If a target is found, set targeting as true and move state to targeting
            if (nearestTarget != null)
            {
                currentlyTargeting = true;
                movementScript.ToggleTargeting(currentlyTargeting, nearestTarget);
                ToggleTargetVisual();
                stateOfTargeting = TargetState.Targeting;
            }
        }

        void RemoveTargeting()
        {
            currentlyTargeting = false;
            movementScript.ToggleTargeting(currentlyTargeting, nearestTarget);
            stateOfTargeting = TargetState.NoTarget;
            ToggleTargetVisual();
        }

        void ToggleTargetVisual()
        {
            // Create a empty gameobject holder
            GameObject position;

            // If we there currently is a target, pass it to the empty holder
            if (nearestTarget != null)
                position = nearestTarget.gameObject;
            else
                position = null;

            //Pass the current status and the gameobject holder
            targetCanvas.GetComponent<PlaceUIOnObject>().ToggleTargetUI(currentlyTargeting, position);
        }
        #endregion
    }
}