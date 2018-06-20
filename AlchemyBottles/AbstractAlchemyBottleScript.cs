using UnityEngine;
namespace CloudsOfAvarice
{
    public abstract class AbstractAlchemyBottleScript : MonoBehaviour
    {
        #region Variables
        public float radiusToCheck;                     // The radius to check for targets
        public string tagToTarget;                      // The tag on which to search for targets
        public LayerMask layerToCheck;                  // The layer in to check for targets

        float closestDistanceSqr;                       // The closest distance to the target
        Vector3 currentPosition;                        // The current position of this object
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

            // Set max distance
            closestDistanceSqr = radiusToCheck;

            // Set the current position
            currentPosition = transform.position;

            foreach (Collider target in objects)
            {
                if (target.tag == tagToTarget)
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
}