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
    CameraPositionStates currentCameraState;                    // The current state of the camera
    CameraPositionStates previousCameraChangeState;  	        // The previously desired position of the camera
    [SerializeField]
    float followingOrtographicSize;                             // Sets the orthographic size of the camera in this state
    [SerializeField]
    float targetingOrtographicSize;                             // Sets the orthographic size of the camera in this state
    [SerializeField]
    float sprintingOrtographicSize;                             // Sets the orthographic size of the camera in this state
    [SerializeField]
    float interactionOrtographicSize;                           // Sets the orthographic size of the camera in this state

    [Header("Positions of the camera")]
    [SerializeField]
    float timeToChangePosition = 0.3f;                          // The time allowed to take between changing positions.
    [SerializeField]
    Vector3 cameraStandardFollowPosition;                       // The standard position of the camera
    [SerializeField]
    Vector3 cameraTargetingPosition;                            // The position when targeting an enemy
    [SerializeField]
    Vector3 cameraInteractionPosition;                          // The position when interacting with something
    [SerializeField]
    Vector3 offset;                                             // The initial offset from the target.
    [SerializeField]
    Vector3 targetingOffset;					                // The offset from the standard position when targeting

    [Header("Deadzone management")]
    [SerializeField]
    bool useDeadzone;                       			        // If deadzone should be used for the camera.
    [SerializeField]
    float deadzoneHeight;					                    // The height of the deadzone
    [SerializeField]
    float deadzoneWidth;					                    // The width of the deadzone

    //Internal variables
    Vector3 angularVelocity;                                    // The current velocity of the camera.
    Camera cameraComponent;                                     // Reference to camera to change ortographic size

    float ortographicSizeToCheck;                               // Holder for currently needed ortographic size.
    Vector3 cameraPositionToCheck;                              // Holder for the currently needed camera position.

    bool cameraOrtographicChangeComplete;		                // Bool to check if the ortographic change is completed
    bool cameraPositionChangeComplete;		        	        // Bool to check if the position change is completed

    bool switchedState;						                    // Bool to check if state change is ready
    CameraPositionChangeState currentCameraChangeState;         // The current desired position of the camera

    public CameraPositionStates testcam;
    #endregion

    #region Camera interface
    public void SetCameraState(CameraPositionStates switchToState)
    {
        // Check to make sure it is a new state
        if (currentCameraChangeState == CameraPositionChangeState.Ready && switchToState != currentCameraState)
        {

            if (currentCameraState == CameraPositionStates.Sprinting)
                currentCameraState = previousCameraChangeState;
            else
                currentCameraState = switchToState;

            if (switchToState != CameraPositionStates.Sprinting)
                previousCameraChangeState = currentCameraState;

            switchedState = true;
        }
    }
    #endregion

    #region Unity standard functions
    // Use this for initialization
    void Start()
    {
        // Set the camera position offset variables.
        // offset = transform.position - targetPosition.position;
        cameraStandardFollowPosition = targetPosition.position + offset;
        cameraTargetingPosition = cameraStandardFollowPosition + targetingOffset;
        cameraComponent = GetComponent<Camera>();

        // Initial camera state.
        switchedState = false;
        SetCameraPosition(CameraPositionStates.Following);
        previousCameraChangeState = CameraPositionStates.Following;
    }

    // Update is called once per frame
    void Update()
    {
        SetCameraState(testcam);

        //Update camera positions
        cameraStandardFollowPosition = targetPosition.position + offset;
        cameraTargetingPosition = cameraStandardFollowPosition + targetingOffset;

        // Move the camera
        ManageCameraPosition();
    }

    //Called on a fixed interval
    private void FixedUpdate()
    {

    }
    #endregion


    #region Change camera size and position
    void ManageCameraPosition()
    {
        switch (currentCameraChangeState)
        {
            case CameraPositionChangeState.Ready:
                if (switchedState)
                {
                    SetCameraPosition();
                    StartCoroutine(ChangeOrthographicSize(cameraComponent.orthographicSize, ortographicSizeToCheck, timeToChangePosition));
                    StartCoroutine(MoveCamera(transform.position, cameraPositionToCheck, timeToChangePosition));
                    currentCameraChangeState = CameraPositionChangeState.Changing;
                }
                else
                {
                    if (useDeadzone)
                        ManageCameraDeadzonePosition();
                    else
                        MoveCamera();
                }
                break;
            case CameraPositionChangeState.Changing:
                if (cameraOrtographicChangeComplete) // Doesn't check if position is true before switching.
                    currentCameraChangeState = CameraPositionChangeState.Verifying;
                break;
            case CameraPositionChangeState.Verifying:
                if (Vector3.Distance(transform.position, cameraPositionToCheck) > 5f)
                    transform.position = Vector3.Lerp(transform.position, cameraPositionToCheck, (followSmoothing / 2f) * Time.deltaTime);
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
    #region Deadzone management
    void ManageCameraDeadzonePosition()
    {
        //  SetCameraPosition(CameraPositionStates.Deadzone);
        // Set deadzone x,y
        float x = (Screen.width - deadzoneWidth) * 0.5f;
        float y = (Screen.height - deadzoneHeight) * 0.5f;

        // Create deadzone rectangle
        Rect deadzoneArea = new Rect(x, y, deadzoneWidth, deadzoneHeight);

        // Get target Worldspace to UI
        Vector3 targetPositionOnScreen = cameraComponent.WorldToScreenPoint(targetPosition.position);

        // Check if target is outside deadzone
        if (!deadzoneArea.Contains(targetPositionOnScreen))
        {
            Debug.Log("Moving outside of deadzone");
            SetCameraState(CameraPositionStates.Following);
        }

    }
    #endregion

    void MoveCamera()
    {
        SetCameraPosition();
        transform.position = Vector3.Lerp(transform.position, cameraPositionToCheck, followSmoothing * Time.deltaTime);
        SmoothCenterOnTarget();
    }

    void SmoothCenterOnTarget()
    {
        transform.LookAt(targetPosition);
    }

    #region CameraPosition and ortographic size management
    void SetCameraPosition()
    {
        switch (currentCameraState)
        {
            case CameraPositionStates.Following:
                SetOrtographicSize(currentCameraState);
                SetCameraPosition(currentCameraState);
                break;
            case CameraPositionStates.Targeting:
                SetOrtographicSize(currentCameraState);
                SetCameraPosition(currentCameraState);
                break;
            case CameraPositionStates.Sprinting:
                SetOrtographicSize(currentCameraState);
                SetCameraPosition(previousCameraChangeState);
                break;
            case CameraPositionStates.Interacting:
                SetOrtographicSize(currentCameraState);
                SetCameraPosition(currentCameraState);
                break;
        }
    }

    void SetOrtographicSize(CameraPositionStates ortographicSize)
    {
        switch (ortographicSize)
        {
            case CameraPositionStates.Following:
                ortographicSizeToCheck = followingOrtographicSize;
                break;
            case CameraPositionStates.Targeting:
                ortographicSizeToCheck = targetingOrtographicSize;
                break;
            case CameraPositionStates.Sprinting:
                ortographicSizeToCheck = sprintingOrtographicSize;
                break;
            case CameraPositionStates.Interacting:
                ortographicSizeToCheck = targetingOrtographicSize;
                break;
        }
    }

    void SetCameraPosition(CameraPositionStates cameraPosition)
    {
        // Camera positions
        switch (cameraPosition)
        {
            case CameraPositionStates.Following:
                cameraPositionToCheck = cameraStandardFollowPosition;
                break;
            case CameraPositionStates.Targeting:
                cameraPositionToCheck = cameraTargetingPosition;
                break;
            case CameraPositionStates.Sprinting:
                cameraPositionToCheck = cameraStandardFollowPosition;
                break;
            case CameraPositionStates.Interacting:
                cameraPositionToCheck = cameraTargetingPosition;
                break;
        }
    }
    #endregion

    IEnumerator ChangeOrthographicSize(float currentSize, float changeToSize, float duration)
    {
        float elapsedTime = 0;
        cameraOrtographicChangeComplete = false;
        Debug.Log("Change ortographic size called");
        while (elapsedTime < duration || cameraComponent.orthographicSize != changeToSize)
        {
            cameraComponent.orthographicSize = Mathf.Lerp(currentSize, changeToSize, (elapsedTime / duration));
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
        while (elapsedTime < duration || Vector3.Distance(transform.position, toPosition) > 0.05f)
        {
            transform.position = Vector3.Lerp(fromPosition, toPosition, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;

            if (Vector3.Distance(transform.position, toPosition) < 0.1f)
                transform.position = toPosition;

            transform.LookAt(targetPosition);
            yield return 0;
        }
        cameraPositionChangeComplete = true;
    }
    #endregion
}
