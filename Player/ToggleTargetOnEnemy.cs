using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleTargetOnEnemy : MonoBehaviour
{
    #region Variables
    public float radiusToCheck;                     // The radius to check for targets
    public float distanceToRemoveTargeting;         // If object is too far away, remove targeting.
    public GameObject targetCanvas;

    Transform nearestTarget;                        // The transform position of the nearest target
    float closestDistanceSqr;                       // The closest distance to the target
    Vector3 currentPosition;                        // The current position of this object
    bool stateOfTargeting;                          // The bool to check if targeting is already applied
    IMovement movementScript;                       // Reference for the movement script to control turning
    public LayerMask layerToCheck;                  // The layer in to check for targets
    #endregion

    void Start()
    {
        stateOfTargeting = false;
        movementScript = GetComponent<IMovement>();
        nearestTarget = null;
    }

    void Update()
    {
        if (Input.GetButtonDown(Inputs.TARGET))
            ToggleTargeting();

        if (stateOfTargeting && Vector3.Distance(nearestTarget.position, transform.position) > distanceToRemoveTargeting)
            RemoveTargeting();
    }

    void ToggleTargeting()
    {
        if (!stateOfTargeting)
            FindAndTargetClosestObjectByLayerAndTag();
        else
            RemoveTargeting();
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
            stateOfTargeting = true;
            movementScript.ToggleTargeting(true, nearestTarget);
            Debug.Log("Target acquired");
            ShowTargetVisual();
        }
    }

    void RemoveTargeting()
    {
        Debug.Log("Target too far away");
        movementScript.ToggleTargeting(false, nearestTarget);
        nearestTarget = null;
        stateOfTargeting = false;
        RemoveTargetVisual();
    }

    #region Visuals
    void ShowTargetVisual()
    {
        targetCanvas.GetComponent<PlaceUIOnObject>().ToggleTargetUI(true, nearestTarget.gameObject);
    }
    void RemoveTargetVisual()
    {
        targetCanvas.GetComponent<PlaceUIOnObject>().ToggleTargetUI(false, null);
    }
    #endregion
}
