using UnityEngine;
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
        public bool retargetOnTargetDeath;              // If a new target should automatically be picked on first targets death.

        [Header("Setup for canvas, UI element to display and camera")]
        public Canvas completeViewportCanvas;           // This needs to be the complete screen.
        public GameObject objectToDisplay;              // Which gameobject to display when targeting.
        public Camera cameraToShowTargetFrom;           // Which camera to get the viewport from. 

        //Internal variables
        RectTransform completeScreenRectTransform;      // The entire screen viewport.
        Transform nearestEntityToTarget;                // The transform position of the nearest target.

        bool currentlyTargeting;                        // Bool for keeping track of targeting is active.

        // Which entity to track.
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
                CheckTargetEntityStatus();
            }
            else
            {
                if (Input.GetButtonDown(Inputs.TARGET))
                    FindAndTargetClosestEntityByLayerAndTag();
            }
        }

        void PlaceAndMaintainUIOnTarget()
        {
            if (nearestEntityToTarget != null)
                objectToDisplay.transform.position = WorldToCanvasPosition(nearestEntityToTarget.position);
        }

        //TODO Find a more performant solution
        void CheckTargetEntityStatus()
        {
            if (nearestEntityToTarget != null)
            {
                if (IsTargetEntityAlive())
                {
                    if (Vector3.Distance(nearestEntityToTarget.position, localEntity.transform.position) > distanceToRemoveTargeting || Input.GetButtonDown(Inputs.TARGET))
                        RemoveTargeting();
                }
                else if (retargetOnTargetDeath)
                    FindAndTargetClosestEntityByLayerAndTag();
                else
                    RemoveTargeting();
            }
            else
                RemoveTargeting();
        }

        //TODO Get refrence to entity health, check if it's alive.
        bool IsTargetEntityAlive()
        {
            return true;
        }
        #endregion

        #region Visual management
        void FindAndTargetClosestEntityByLayerAndTag()
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

            // If a target is found, set targeting as true and move state to targeting. Otherwise remove targeting
            if (nearestEntityToTarget != null)
                ApplyTargeting();
            else
                RemoveTargeting();
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
            // Clear nearest entity if targeting is not active
            if (!currentlyTargeting)
                nearestEntityToTarget = null;

            // Call movement script to set lookAt() true/false and current target
            movementScript.ToggleTargeting(currentlyTargeting, nearestEntityToTarget);

            // Set targeting visual object true/false
            objectToDisplay.SetActive(currentlyTargeting);
        }

        //Translate the target position from worldspace to canvas space
        Vector3 WorldToCanvasPosition(Vector3 positionOfTarget)
        {
            //Get worldspace(Vector3) from camera and translate to canvas (Vector2)
            Vector2 positionOnCanvasFromWorldSpace = cameraToShowTargetFrom.WorldToViewportPoint(positionOfTarget);

            //Calculate position considering percentage, using our canvas size
            //If canvas size is (1100,500), and percentage is (0.5,0.5), current value will be (550,250)
            positionOnCanvasFromWorldSpace.x *= completeScreenRectTransform.sizeDelta.x;
            positionOnCanvasFromWorldSpace.y *= completeScreenRectTransform.sizeDelta.y;

            return positionOnCanvasFromWorldSpace;
        }
        #endregion
    }
}