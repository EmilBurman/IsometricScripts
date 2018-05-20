using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPotion : MonoBehaviour
{
    #region Variables
    public float radiusToCheck;                     // The radius to check for targets
    public string tagToTarget;                      // The tag on which to search for targets
    public LayerMask layerToCheck;                  // The layer in to check for targets
    bool initialCollision;                          // Checks if this is the first collision
    #endregion

    void Start()
    {
        if (tagToTarget == "")
            tagToTarget = Tags.ENEMY;
        initialCollision = true;
    }

    #region Target management
    void OnCollisionEnter(Collision col)
    {
        if (initialCollision)
        {
            BreakOnImpact();
            SplatterOnImpact();
            FindTargetsByLayerAndTag();
            initialCollision = false;
        }
    }

    void FindTargetsByLayerAndTag()
    {
        //Get objects in radius by layer
        Collider[] objects = Physics.OverlapSphere(transform.position, radiusToCheck, layerToCheck);

        foreach (Collider target in objects)
        {
            if (target.tag == tagToTarget)
            {
                SetStatus(target);
            }
        }

        // After all targets with correct tags are set as hit, remove this game object
        DeactivateGameObject();
    }

    void SetStatus(Collider targetToSetStatus)
    {
        Debug.Log(targetToSetStatus.tag);
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
