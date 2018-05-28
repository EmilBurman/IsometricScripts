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
    cameraState currentCameraState;                             // The current state of the camera
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
    Vector3 cameraTargetingPosition;                                  // The position when targeting an enemy
    [SerializeField]
    Vector3 cameraInteractionPosition;                                // The position when interacting with something

    //Internal variables
    Vector3 offset;                                             // The initial offset from the target.
    Vector3 angularVelocity;                                    // The current velocity of the camera.
    Camera cameraComponent;
    float ortographicSizeToCheck;                               // Holder for currently needed ortographic size.
    Vector3 cameraPositionToCheck;                              // Holder for the currently needed camera position.
    bool switchedState;
    public cameraState testcamera;
    #endregion

    #region Camera interface
    public void SetCameraState(cameraState switchToState)
    {
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
        SetCameraState(testcamera);
        cameraStandardFollowPosition = targetPosition.position + offset;
        cameraTargetingPosition = cameraStandardFollowPosition + new Vector3(-9, 21, -25);
        ChangePosition(currentCameraState);
        SmoothCenterOnTarget();
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

    void ChangePosition(cameraState changeToState)
    {
        switch (changeToState)
        {
            case cameraState.Following:
                ortographicSizeToCheck = followingOrtographicSize;
                cameraPositionToCheck = cameraStandardFollowPosition;
                break;
            case cameraState.Targeting:
                ortographicSizeToCheck = targetingOrtographicSize;
                cameraPositionToCheck = cameraTargetingPosition;
                break;
            case cameraState.Sprinting:
                ortographicSizeToCheck = sprintingOrtographicSize;
                break;
        }

        if (cameraComponent.orthographicSize != ortographicSizeToCheck && switchedState)
            StartCoroutine(ChangeOrthographicSize(cameraComponent.orthographicSize, ortographicSizeToCheck, timeToChangePosition));

        if ((Vector3.Distance(transform.position, cameraPositionToCheck) > 20f && switchedState) || switchedState)
            StartCoroutine(MoveCamera(transform.position, cameraPositionToCheck, timeToChangePosition));
        else
            transform.position = Vector3.Lerp(transform.position, cameraPositionToCheck, followSmoothing * Time.deltaTime);
    }

    IEnumerator ChangeOrthographicSize(float currentSize, float changeToSize, float duration)
    {
        float elapsedTime = 0;
        Debug.Log("Change ortographic size called");
        while (elapsedTime < duration)
        {
            cameraComponent.orthographicSize = Mathf.Lerp(currentSize, changeToSize, elapsedTime);
            elapsedTime += Time.deltaTime;
            yield return 0;
        }
        switchedState = false;
    }

    IEnumerator MoveCamera(Vector3 fromPosition, Vector3 toPosition, float duration)
    {
        //create float to store the time this coroutine is operating
        float elapsedTime = 0;
        Debug.Log("Move camera called");
        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(fromPosition, toPosition, elapsedTime);
            elapsedTime += Time.deltaTime;
            yield return 0;
        }
        switchedState = false;
    }
}
