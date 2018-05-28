using StateEnumerators;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleTargetOnEnemy : MonoBehaviour
{
    #region Variables
    public float radiusToCheck;                     // The radius to check for targets
    public float distanceToRemoveTargeting;         // If object is too far away, remove targeting
    public GameObject targetCanvas;                 // Where to find script that controls target circle
    public string tagToTarget;                      // The tag on which to search for targets
    public LayerMask layerToCheck;                  // The layer in to check for targets

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
                {
                    FindAndTargetClosestObjectByLayerAndTag();
                }
                break;
            case TargetState.Targeting:
                if (Vector3.Distance(nearestTarget.position, transform.position) > distanceToRemoveTargeting || Input.GetButtonDown(Inputs.TARGET))
                    RemoveTargeting();
                if (Input.GetButtonDown(Inputs.RETARGET))
                    FindAndTargetClosestObjectByLayerAndTag();
                break;
        }
    }

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

        // If a target is found, set targeting as true
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
        nearestTarget = null;
        movementScript.ToggleTargeting(currentlyTargeting, nearestTarget);
        stateOfTargeting = TargetState.NoTarget;
        ToggleTargetVisual();
    }

    void ToggleTargetVisual()
    {
        GameObject position;

        if (nearestTarget != null)
            position = nearestTarget.gameObject;
        else
            position = null;

        targetCanvas.GetComponent<PlaceUIOnObject>().ToggleTargetUI(currentlyTargeting, position);
    }
}
