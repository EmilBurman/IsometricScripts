using CloudsOfAvarice.StateEnumerators;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace CloudsOfAvarice
{
    public class ThreeStageWorldUIPlacement : MonoBehaviour
    {
        #region Variables
        [Header("UI Image setup.")]
        public Image pointOfInterestUIElement;
        public Image informationPromptUIElement;
        public Image interactionPromptUIElement;

        [Header("Minimum distance for each UI stage.")]
        public float minDistPointOfInterest;
        public float minDistInformationPrompt;
        public float minDistInteractionPrompt;

        [Header("The time it takes to interact before interaction animation is displayed.")]
        public float timeForInteracting;


        // Internal variables
        Transform localPlayer;
        IController3D localPlayerInteracting;
        Vector3 placementPositionForUI;
        Animator firstStageAnimator;
        Animator secondStageAnimator;
        Animator thirdStageAnimator;
        WorldUiState uiState;

        float time;
        #endregion

        #region Initalization
        void Start()
        {
            localPlayer = null;
            placementPositionForUI = pointOfInterestUIElement.transform.position;
            firstStageAnimator = pointOfInterestUIElement.gameObject.GetComponent<Animator>();
            secondStageAnimator = informationPromptUIElement.gameObject.GetComponent<Animator>();
            thirdStageAnimator = interactionPromptUIElement.gameObject.GetComponent<Animator>();
            DeactivateAllPrompts();
        }

        void Update()
        {
            if (localPlayer != null)
            {
                ManageUiState();
                RotateToFacePlayer();
            }
        }
        #endregion

        #region Trigger functions
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
                uiState = WorldUiState.NoUi;
                localPlayer = null;
                DeactivateAllPrompts();
                Debug.Log("Local player exited the area.");
            }
        }

        void TriggerInteractionEvent()
        {

        }
        #endregion

        #region Player management
        bool CheckIfLocalPlayer()
        {
            return true;
        }

        void SetLocalPlayer(Collider entity)
        {
            localPlayer = entity.transform;
            localPlayerInteracting = entity.GetComponent<IController3D>();
        }

        bool CheckIfPlayerIsInteracting()
        {
            return localPlayerInteracting.Interact();
        }

        float DistanceToPlayer()
        {
            if (localPlayer != null)
                return Vector3.Distance(placementPositionForUI, localPlayer.position);
            else
                return 500f;
        }

        float AngleToPlayer()
        {
            return Vector3.SignedAngle(placementPositionForUI, localPlayer.position, Vector3.right);
        }
        #endregion

        #region UiStateFunctions
        private void ManageUiState()
        {
            //Debug.Log(DistanceToPlayer());
            switch (uiState)
            {
                case WorldUiState.NoUi:
                    if (DistanceToPlayer() < minDistPointOfInterest)
                    {
                        ManagePointOfInterestImage(true);
                        uiState = WorldUiState.Poi;
                    }
                    break;
                case WorldUiState.Poi:
                    if (DistanceToPlayer() < minDistInformationPrompt)
                    {
                        ManageInformationImage(true);
                        uiState = WorldUiState.InfoPrompt;
                    }
                    else if (DistanceToPlayer() > minDistPointOfInterest)
                    {
                        ManagePointOfInterestImage(false);
                        DeactivateAllPrompts();
                        uiState = WorldUiState.NoUi;
                    }
                    break;
                case WorldUiState.InfoPrompt:
                    if (DistanceToPlayer() < minDistInteractionPrompt)
                    {
                        ManageInteractionImage(true);
                        uiState = WorldUiState.InteractionPrompt;
                    }
                    else if (DistanceToPlayer() > minDistInformationPrompt)
                    {
                        ManageInformationImage(false);
                        uiState = WorldUiState.Poi;
                    }
                    break;
                case WorldUiState.InteractionPrompt:
                    if (CheckIfPlayerIsInteracting())
                    {
                        DisplayInteractionAnimation(true);
                    }
                    else
                        DisplayInteractionAnimation(false);

                    if (DistanceToPlayer() > minDistInteractionPrompt)
                    {
                        DisplayInteractionAnimation(false);
                        ManageInteractionImage(false);
                        uiState = WorldUiState.InfoPrompt;
                    }
                    break;
                case WorldUiState.DeactivateUi:
                    DestroyThisGameObject();
                    break;
            }
        }

        void ManagePointOfInterestImage(bool status)
        {
            firstStageAnimator.SetBool("Active", status);
            Debug.Log("Showing Poi");
        }

        void ManageInformationImage(bool status)
        {
            secondStageAnimator.SetBool("Active", status);
            Debug.Log("Showing InformationPrompt");
        }

        void ManageInteractionImage(bool status)
        {
            thirdStageAnimator.SetBool("Active", status);
            Debug.Log("Showing InteractionPrompt");
        }

        void DisplayInteractionAnimation(bool status)
        {
            thirdStageAnimator.SetBool("Interacting", status);
            if (status && time == 0)
                StartCoroutine(InteractionTimer());
            Debug.Log("Can interact with object");
        }

        IEnumerator InteractionTimer()
        {
            time = 0;
            while (time < timeForInteracting && CheckIfPlayerIsInteracting() && DistanceToPlayer() < minDistInteractionPrompt)
            {
                time += Time.fixedDeltaTime;
                Debug.Log(time);

                yield return 0;
            }

            if (time > timeForInteracting)
                uiState = WorldUiState.DeactivateUi;
            else
                time = 0;
        }

        void DestroyThisGameObject()
        {
            TriggerInteractionEvent();
            thirdStageAnimator.SetBool("InteractionDone", true);
            Destroy(this.gameObject);
        }

        void DeactivateAllPrompts()
        {
            ManagePointOfInterestImage(false);
            ManageInformationImage(false);
            ManageInteractionImage(false);
            Debug.Log("Deactivating Ui");
        }

        void RotateToFacePlayer()
        {
            if (AngleToPlayer() < 10f)
            {
                Debug.Log(AngleToPlayer());
            }
        }
        #endregion
    }
}