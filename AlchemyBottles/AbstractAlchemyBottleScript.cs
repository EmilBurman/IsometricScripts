using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractAlchemyBottleScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollision(colider col)
    {

    }

    #region Variables
    public string tagsToCheck;
    #endregion

    #region Target checks
    void FindTargetsByLayerAndTag()
    {
        //Get objects in radius by layer
        Collider[] objects = Physics.OverlapSphere(transform.position, radiusToCheck, layerToCheck);

        // Set max distance
        closestDistanceSqr = radiusToCheck;

        // Set the current position
        currentPosition = transform.position;

        foreach (Collider target in objects)
        {
            if (target.tag == Tags.ENEMY)
            {
                //Set them as hit by bottle
            }
        }

        // If a target is found, set targeting as true
        if (nearestTarget != null)
        {
            currentlyTargeting = true;
            movementScript.ToggleTargeting(currentlyTargeting, nearestTarget);
            ToggleTargetVisual();
        }
    }
    #endregion
}
