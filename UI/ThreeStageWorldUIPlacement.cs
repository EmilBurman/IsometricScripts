using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreeStageWorldUIPlacement : MonoBehaviour
{
    #region Variables
    [Header("UI Image setup")]
    public GameObject pointOfInterestUIElement;
    public GameObject informationPromptUIElement;
    public GameObject interactionPromptUIElement;

    [Header("Minimum distance for each UI stage")]
    public float minDistPointOfInterest;
    public float minDistInformationPrompt;
    public float minDistInteractionPrompt;

    Transform entityPosition;
    float distanceToEntity;
    Transform placementPositionForUI;
    #endregion

    #region Initalization
    // Use this for initialization
    void Start()
    {
        /* This should check if there are three scripts attached to this gameobject
         * Point of interest
         * Information prompt
         * interaction prompt
         * 
         * If all three are here, a three staged rocked should occur, otherwise
         * They should be displayed as they are attached.
         * For instance the point of interest UI element might not be necessary
         */

    }

    void Awake()
    {
        /* UI PLACEMENT
         * Get parent transform
         * if their Y length isnt greater than MAX SIZE
         * place UI start position ( middle of parentObject)
         * else place on side towards negative X just outside of parent object collider
         *
         * TRIGGER BOX PLACEMENT
         * place standard box 22 X, 4 Y, 22 Z
         * 
         *
         *
         */
    }
    #endregion

    void OnTriggerEnter(Collider entity)
    {
        if (entity.CompareTag(Tags.PLAYER))
        {
            if (playerPosition = null)
                playerPosition = entity.transform;

            /* RAYCAST TO PLAYER POSITION
            * Raycast from parentObject towards player
            * if no terrain obstacles and if within MIN RANGE
            * place ShowPointOfInterestPrompt
            * if within MIN RANGE for ShowObjectInformationPrompt
            * ShowObjectInformationPrompt
            * if within MIN RANGE
            * ShowInteractionPrompt
            */



            Debug.Log("Player entered area.");
        }
    }

    private void OnTriggerStay(Collider entity)
    {
        if (playerPosition != null)
        {
            Debug.Log("Holys hit its the player");
        }
    }

    void ShowPointOfInterestPrompt()
    {

    }

    void ShowObjectInformationPrompt()
    {

    }

    void ShowInteractionPrompt()
    {

    }

    void OnTriggerExit(Collider entity)
    {
        if (playerPosition = entity.transform)
        {
            playerPosition = null;
            Debug.Log("Player exited area.");
        }
    }
}
