using CloudsOfAvarice.StateEnumerators;
using UnityEngine;
using UnityEngine.UI;

namespace CloudsOfAvarice
{
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
        Transform placementPositionForUI;
        Animator animatorForUI;
        float distanceToEntity;
        WorldUiState uiState;
        bool displayPointOfInterestImage;
        bool displayInformationImage;
        bool displayInteractionImage;

        #endregion

        #region Initalization
        void Start()
        {
            alphaMax.a = 1f;
            alphaMin.a = 0f;
            placementPositionForUI = this.transform;
            animatorForUI = GetComponent<Animator>();
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
                        displayPointOfInterestImage = true;
                        ManagePointOfInterestImage();
                        uiState = WorldUiState.Poi;
                    }
                    break;
                case WorldUiState.Poi:
                    if (DistanceToPlayer() < minDistInformationPrompt)
                    {
                        displayInformationImage = true;
                        ManageInformationImage();
                        uiState = WorldUiState.InfoPrompt;
                    }
                    else if (DistanceToPlayer() > minDistPointOfInterest)
                    {
                        displayInformationImage = false;
                        ManagePointOfInterestImage();
                        RemoveUI();
                        uiState = WorldUiState.NoUi;
                    }
                    break;
                case WorldUiState.InfoPrompt:
                    if (DistanceToPlayer() < minDistInteractionPrompt)
                    {
                        displayInteractionImage = true;
                        ManageInteractionImage();
                        uiState = WorldUiState.InteractionPrompt;
                    }
                    else if (DistanceToPlayer() > minDistInformationPrompt)
                    {
                        displayInformationImage = false;
                        ManageInformationImage();
                        uiState = WorldUiState.Poi;
                    }
                    break;
                case WorldUiState.InteractionPrompt:
                    if (localPlayer.GetComponent<IController3D>().Interact())
                    {
                        DisplayInteractionAnimation();
                        uiState = WorldUiState.Interacting;
                    }
                    if (DistanceToPlayer() > minDistInteractionPrompt)
                    {
                        displayInteractionImage = false;
                        ManageInteractionImage();
                        uiState = WorldUiState.InfoPrompt;
                    }
                    break;
                case WorldUiState.Interacting:
                    //this.gameObject(deactive);
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
        void ManagePointOfInterestImage()
        {
            animatorForUI.SetBool("WithinPointOfInterestRange", displayPointOfInterestImage);
            pointOfInterestUIElement.color = alphaMax;
            Debug.Log("Showing Poi");
        }


        void ManageInformationImage()
        {
            animatorForUI.SetBool("WithinInformationRange", displayInformationImage);
            informationPromptUIElement.color = alphaMax;
            Debug.Log("Showing InformationPrompt");
        }

        void ManageInteractionImage()
        {
            animatorForUI.SetBool("WithinInteractionRange", displayInteractionImage);
            interactionPromptUIElement.color = alphaMax;
            Debug.Log("Showing InteractionPrompt");
        }

        void DisplayInteractionAnimation()
        {
            animatorForUI.SetBool("InteractingWithObject", localPlayer.GetComponent<IController3D>().Interact());
            //interactionPromptUIElement.color = alphaMax;
            Debug.Log("Showing InteractionPrompt");
        }

        void RemoveUI()
        {
            pointOfInterestUIElement.color = alphaMin;
            informationPromptUIElement.color = alphaMin;
            interactionPromptUIElement.color = alphaMin;
            Debug.Log("Removing Ui");
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
}