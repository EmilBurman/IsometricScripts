using StateEnumerators;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class IsometricCamera : MonoBehaviour
{
    #region Variables
    [Header("Target and smoothing options")]
    public Transform targetPosition;                            // The position that that camera will be following.
    public float followSmoothing = 5f;                          // The speed with which the camera will be following.
    public float rotationSmoothing = 5f;                        // The speed with which the camera will be rotating.

    [Header("Ortographic views of the camera")]
    [SerializeField]
    CameraPositionStates currentCameraState;                             // The current state of the camera
    [SerializeField]
    float followingOrtographicSize;                             // Sets the orthographic size of the camera in this state
    [SerializeField]
    float targetingOrtographicSize;                             // Sets the orthographic size of the camera in this state
    [SerializeField]
    float sprintingOrtographicSize;                             // Sets the orthographic size of the camera in this state
    [SerializeField]
    float interactionOrtographicSize;                           // Sets the orthographic size of the camera in this state
    [SerializeField]
    float timeToChangePosition = 0.3f;                          // The time allowed to take between changing positions.

    [Header("Positions of the camera")]
    [SerializeField]
    Vector3 cameraStandardFollowPosition;                       // The standard position of the camera
    [SerializeField]
    Vector3 cameraTargetingPosition;                            // The position when targeting an enemy
    [SerializeField]
    Vector3 cameraInteractionPosition;                          // The position when interacting with something

    //Internal variables
    Vector3 offset;                                             // The initial offset from the target.
    Vector3 angularVelocity;                                    // The current velocity of the camera.
    Camera cameraComponent;                                     // Reference to camera to change ortographic size
    float ortographicSizeToCheck;                               // Holder for currently needed ortographic size.
    Vector3 cameraPositionToCheck;                              // Holder for the currently needed camera position.

    bool cameraOrtographicChangeComplete;
    bool cameraPositionChangeComplete;


    bool switchedState;
    public CameraPositionChangeState currentCameraChangeState;
    TwoStageState movingCameraState;
    public CameraPositionStates testcam;
    #endregion

    #region Camera interface
    public void SetCameraState(CameraPositionStates switchToState)
    {
        if (currentCameraChangeState == CameraPositionChangeState.Ready)
            if (switchToState != currentCameraState)
            {
                currentCameraState = switchToState;
                switchedState = true;
            }
    }
    #endregion

    #region Unity standard functions
    // Use this for initialization
    void Start()
    {
        switchedState = false;
        cameraStandardFollowPosition = targetPosition.position + offset;
        cameraTargetingPosition = cameraStandardFollowPosition + new Vector3(-9, 21, -25);
        offset = transform.position - targetPosition.position;
        cameraComponent = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        SetCameraState(testcam);
        cameraStandardFollowPosition = targetPosition.position + offset;
        cameraTargetingPosition = cameraStandardFollowPosition + new Vector3(-9, 21, -25);
        ChangePosition(currentCameraState);
    }

    //Called on a fixed interval
    private void FixedUpdate()
    {

    }
    #endregion

    void SmoothCenterOnTarget()
    {
        transform.LookAt(targetPosition);
    }


    //Needs mayor rewriting
    void ChangePosition(CameraPositionStates cameraPosition)
    {

        // Camera positions
        switch (cameraPosition)
        {
            case CameraPositionStates.Following:
                ortographicSizeToCheck = followingOrtographicSize;
                cameraPositionToCheck = cameraStandardFollowPosition;
                break;
            case CameraPositionStates.Targeting:
                ortographicSizeToCheck = targetingOrtographicSize;
                cameraPositionToCheck = cameraTargetingPosition;
                break;
            case CameraPositionStates.Sprinting:
                // Need to figure out a way to get and set camera position.
                ortographicSizeToCheck = sprintingOrtographicSize;
                break;
        }

        // State of change for the camera
        switch (currentCameraChangeState)
        {
            case CameraPositionChangeState.Ready:
                if (switchedState)
                {
                    StartCoroutine(ChangeOrthographicSize(cameraComponent.orthographicSize, ortographicSizeToCheck, timeToChangePosition));
                    StartCoroutine(MoveCamera(transform.position, cameraPositionToCheck, timeToChangePosition));
                    currentCameraChangeState = CameraPositionChangeState.Changing;
                }
                else
                {
                    transform.position = Vector3.Lerp(transform.position, cameraPositionToCheck, followSmoothing * Time.deltaTime);
                    SmoothCenterOnTarget();
                }
                break;
            case CameraPositionChangeState.Changing:
                if (cameraOrtographicChangeComplete) // Doesn't check if position is true before switching.
                    currentCameraChangeState = CameraPositionChangeState.Verifying;
                break;
            case CameraPositionChangeState.Verifying:
                if (Vector3.Distance(transform.position, cameraPositionToCheck) > 5f)
                    transform.position = Vector3.Lerp(transform.position, cameraPositionToCheck, followSmoothing * 2f * Time.deltaTime);
                else if (Mathf.Abs(cameraComponent.orthographicSize - ortographicSizeToCheck) > 0.005f)
                    cameraComponent.orthographicSize = ortographicSizeToCheck;
                else
                {
                    switchedState = false;
                    currentCameraChangeState = CameraPositionChangeState.Ready;
                }
                break;
        }
    }
    #region Courutines for changing camera position
    IEnumerator ChangeOrthographicSize(float currentSize, float changeToSize, float duration)
    {
        float elapsedTime = 0;
        cameraOrtographicChangeComplete = false;
        Debug.Log("Change ortographic size called");
        while (elapsedTime < duration || cameraComponent.orthographicSize != changeToSize)
        {
            cameraComponent.orthographicSize = Mathf.Lerp(currentSize, changeToSize, elapsedTime);
            elapsedTime += Time.deltaTime;

            if (Mathf.Abs(cameraComponent.orthographicSize - changeToSize) < 0.005f)
                cameraComponent.orthographicSize = changeToSize;
            yield return 0;
        }
        cameraOrtographicChangeComplete = true;
    }

    IEnumerator MoveCamera(Vector3 fromPosition, Vector3 toPosition, float duration)
    {
        float elapsedTime = 0;
        cameraPositionChangeComplete = false;
        Debug.Log("Move camera called");
        while (elapsedTime < duration || Vector3.Distance(transform.position, toPosition) < 0.05f)
        {
            transform.position = Vector3.Lerp(fromPosition, toPosition, elapsedTime);
            elapsedTime += Time.deltaTime;

            if (Vector3.Distance(transform.position, toPosition) < 0.1f)
                transform.position = toPosition;

            transform.LookAt(targetPosition);                   //This is needed to maintain focus on target
            yield return 0;
        }
        cameraPositionChangeComplete = true;
    }
    #endregion
}
