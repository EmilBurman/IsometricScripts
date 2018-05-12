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

    #region Variables
    public string tagsToCheck;
    #endregion

    #region Target management
    void OnCollisionEnter(Collision col)
    {
        FindTargetsByLayerAndTag();
    }

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

        // After all targets with correct tags are set as hit, remove this game object
        DeactivateGameObject();


    }

    void SetStatus()
    {
        // Get status management component and set effect as true
    }
    #endregion
    #region Visual management
    void BreakOnImpact()
    {

    }

    void SplatterOnImpact()
    {

    }

    void DeactivateGameObject()
    {

    }
    #endregion
}
