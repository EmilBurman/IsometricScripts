using StateEnumerators;
using UnityEngine;
using UnityEngine.UI;

public class ThreeStageWorldUIPlacement : MonoBehaviour
{
    #region Variables
    [Header("UI Image setup")]
    public Image pointOfInterestUIElement;
    public Image informationPromptUIElement;
    public Image interactionPromptUIElement;
    Color alphaMax;
    Color alphaMin;

    [Header("Minimum distance for each UI stage")]
    public float minDistPointOfInterest;
    public float minDistInformationPrompt;
    public float minDistInteractionPrompt;

    // Internal variables
    Transform localPlayer;
    float distanceToEntity;
    Transform placementPositionForUI;
    WorldUiState uiState;
    #endregion

    #region Initalization
    void Start()
    {
        alphaMax.a = 1f;
        alphaMin.a = 0f;
        placementPositionForUI = this.transform;
        InstantiateUiElements();
        RemoveUI();
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

    void Update()
    {
        if (localPlayer != null)
            ManageUiState();
    }
    #endregion

    #region Unity trigger functions
    void OnTriggerEnter(Collider entity)
    {
        if (entity.CompareTag(Tags.PLAYER))
        {
            if (CheckIfLocalPlayer())
                SetLocalPlayer(entity);
            Debug.Log("Local player entered area.");
        }
    }

    void OnTriggerExit(Collider entity)
    {
        if (entity.transform == localPlayer)
        {
            localPlayer = null;
            Debug.Log("Local player exited the area.");
        }
    }
    #endregion

    private void ManageUiState()
    {
        //Debug.Log(DistanceToPlayer());
        switch (uiState)
        {
            case WorldUiState.NoUi:
                if (DistanceToPlayer() < minDistPointOfInterest)
                {
                    ShowPointOfInterestPrompt();
                    uiState = WorldUiState.Poi;
                }
                break;
            case WorldUiState.Poi:
                if (DistanceToPlayer() < minDistInformationPrompt)
                {
                    ShowObjectInformationPrompt();
                    uiState = WorldUiState.InfoPrompt;
                }
                else if (DistanceToPlayer() > minDistPointOfInterest)
                {
                    RemoveUI();
                    uiState = WorldUiState.NoUi;
                }
                break;
            case WorldUiState.InfoPrompt:
                if (DistanceToPlayer() < minDistInteractionPrompt)
                {
                    ShowInteractionPrompt();
                    uiState = WorldUiState.InteractionPrompt;
                }
                else if (DistanceToPlayer() > minDistInformationPrompt)
                {
                    ShowPointOfInterestPrompt();
                    uiState = WorldUiState.Poi;
                }
                break;
            case WorldUiState.InteractionPrompt:
                /*
                if (localPlayer.GetComponent<IController3D>().Interact())
                {
                    RemoveUI();
                    uiState = WorldUiState.DeactivateUi;
                }
                */
                if (DistanceToPlayer() > minDistInteractionPrompt)
                {
                    ShowObjectInformationPrompt();
                    uiState = WorldUiState.InfoPrompt;
                }
                break;
            case WorldUiState.DeactivateUi:
                //this.gameObject(deactive);
                break;
        }
    }

    bool CheckIfLocalPlayer()
    {
        return true;
    }

    void SetLocalPlayer(Collider entity)
    {
        localPlayer = entity.transform;
    }

    float DistanceToPlayer()
    {
        return Vector3.Distance(placementPositionForUI.position, localPlayer.position);
    }

    #region UiStateFunctions
    void ShowPointOfInterestPrompt()
    {
        pointOfInterestUIElement.color = alphaMax;
        Debug.Log("Showing Poi");
    }


    void ShowObjectInformationPrompt()
    {
        informationPromptUIElement.color = alphaMax;
        Debug.Log("Showing InformationPrompt");
    }

    void ShowInteractionPrompt()
    {
        interactionPromptUIElement.color = alphaMax;
        Debug.Log("Showing InteractionPrompt");
    }

    void RemoveUI()
    {
        pointOfInterestUIElement.color = alphaMin;
        informationPromptUIElement.color = alphaMin;
        interactionPromptUIElement.color = alphaMin;
        Debug.Log("Removing Ui");
    }

    void InstantiateUiElements()
    {
        pointOfInterestUIElement.transform.position = this.gameObject.transform.position;
        informationPromptUIElement.transform.position = this.gameObject.transform.position;
        interactionPromptUIElement.transform.position = this.gameObject.transform.position;
    }

    void RotateUI()
    {
        /*
        * Raycast towards player
        * raycast towards camera
        * get angle
        * if angle within minAngle and maxAngle
        * rotate ui on x axis, freeze y axis
        * else freeze ui ortation
        */
    }
    #endregion
}