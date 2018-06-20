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
        public bool retargetOnTargetDeath;

        [Header("Setup for canvas, UI element to display and camera")]
        public Canvas completeViewportCanvas;           // This needs to be the complete screen
        public GameObject objectToDisplay;
        public Camera cameraToShowTargetFrom;

        //Internal variables
        RectTransform completeScreenRectTransform;
        GameObject localEntity;
        Transform nearestEntityToTarget;                // The transform position of the nearest target

        bool currentlyTargeting;                        // Bool for keeping track of targeting is active

        IMovement movementScript;                       // Reference for the movement script to control turning for the local entity.
        TargetState stateOfTargeting;                   // The state of targeting
        #endregion

        #region Inital setup
        // Use this for initialization
        void Start()
        {
            FindAndSetLocalPlayer();
            completeScreenRectTransform = completeViewportCanvas.GetComponent<RectTransform>();
            objectToDisplay.SetActive(false);

            if (tagToTarget == "")
                tagToTarget = Tags.ENEMY;

            stateOfTargeting = TargetState.NoTarget;
            movementScript = localEntity.GetComponent<IMovement>();

            currentlyTargeting = false;
            nearestEntityToTarget = null;
        }

        void FindAndSetLocalPlayer()
        {
            localEntity = GameObject.FindGameObjectWithTag(Tags.PLAYER);
        }
        #endregion

        #region Runetime placement
        void FixedUpdate()
        {
            switch (stateOfTargeting)
            {
                case TargetState.NoTarget:
                    if (Input.GetButtonDown(Inputs.TARGET))
                        FindAndTargetClosestObjectByLayerAndTag();
                    break;
                case TargetState.Targeting:
                    PlaceUIOnTarget();
                    if (Vector3.Distance(nearestEntityToTarget.position, localEntity.transform.position) > distanceToRemoveTargeting || Input.GetButtonDown(Inputs.TARGET))
                        RemoveTargeting();
                    if (retargetOnTargetDeath /*&& and check for target health*/)
                        FindAndTargetClosestObjectByLayerAndTag();
                    break;
            }
        }

        void PlaceUIOnTarget()
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
            {
                currentlyTargeting = true;
                stateOfTargeting = TargetState.Targeting;
                PlaceUIOnTarget();
                ToggleTargetVisual();
            }
        }

        void RemoveTargeting()
        {
            currentlyTargeting = false;
            nearestEntityToTarget = null;
            stateOfTargeting = TargetState.NoTarget;
            ToggleTargetVisual();
        }

        void ToggleTargetVisual()
        {
            movementScript.ToggleTargeting(currentlyTargeting, nearestEntityToTarget);
            objectToDisplay.SetActive(currentlyTargeting);
        }

        //Translate the target position from worldspace to canvas space
        Vector3 WorldToCanvasPosition(Vector3 positionOfTarget)
        {
            //Vector position (percentage from 0 to 1) considering camera size.
            //For example (0,0) is lower left, middle is (0.5,0.5)
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