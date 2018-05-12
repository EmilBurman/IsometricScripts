using UnityEngine;
using System.Collections;
using StateEnumerators;

public class CameraFollow : MonoBehaviour
{
    #region Variables
    public Transform target;                    // The position that that camera will be following.
    public float smoothing = 5f;                // The speed with which the camera will be following.

    Vector3 offset;                             // The initial offset from the target.
    public float speedToRotateCamera = 0.3F;


    float finishRotation;
    cameraState currentCameraPosition;
    cameraState cameraState;
    public GameObject cameraPosition1, cameraPosition2, cameraPosition3, cameraPosition4;
    PlayerMovement changeMovementDisplacement;
    #endregion

    void Start()
    {
        // Calculate the initial offset.
        offset = transform.position - target.position;
        cameraState = cameraState.following;
        currentCameraPosition = cameraState.position1;
        changeMovementDisplacement = target.GetComponent<PlayerMovement>();
    }


    void FixedUpdate()
    {

        bool cameraLeft = Input.GetButtonDown(Inputs.SWITCHCAMERALEFT);
        bool cameraRight = Input.GetButtonDown(Inputs.SWITCHCAMERARIGHT);

        switch (cameraState)
        {
            case cameraState.following:
                if (cameraLeft || cameraRight)
                {
                    SwitchCameraPosition(cameraLeft, cameraRight);
                    finishRotation = speedToRotateCamera;
                    cameraState = cameraState.rotating;
                }
                else
                {
                    // Create a postion the camera is aiming for based on the offset from the target.
                    Vector3 targetCamPos = target.position + offset;

                    // Smoothly interpolate between the camera's current position and it's target position.
                    transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
                }
                break;
            case cameraState.rotating:
                finishRotation -= Time.deltaTime;
                if (finishRotation <= 0)
                {
                    finishRotation = 0;
                    cameraState = cameraState.following;
                }

                break;
        }
    }

    void SwitchCameraPosition(bool left, bool right)
    {
        Vector3 toPosition = transform.position;
        Quaternion toRotation = transform.rotation;
        switch (currentCameraPosition)
        {
            case cameraState.position1:
                if (left)
                {
                    toPosition = cameraPosition4.transform.position;
                    toRotation = cameraPosition4.transform.rotation;
                    currentCameraPosition = cameraState.position4;
                }

                if (right)
                {
                    toPosition = cameraPosition2.transform.position;
                    toRotation = cameraPosition2.transform.rotation;
                    currentCameraPosition = cameraState.position2;
                }
                Debug.Log("State 1");
                break;
            case cameraState.position2:
                if (left)
                {
                    toPosition = cameraPosition1.transform.position;
                    toRotation = cameraPosition1.transform.rotation;
                    currentCameraPosition = cameraState.position1;
                }

                if (right)
                {
                    toPosition = cameraPosition3.transform.position;
                    toRotation = cameraPosition3.transform.rotation;
                    currentCameraPosition = cameraState.position3;
                }
                Debug.Log("State 2");
                break;
            case cameraState.position3:
                if (left)
                {
                    toPosition = cameraPosition2.transform.position;
                    toRotation = cameraPosition2.transform.rotation;
                    currentCameraPosition = cameraState.position2;
                }

                if (right)
                {
                    toPosition = cameraPosition4.transform.position;
                    toRotation = cameraPosition4.transform.rotation;
                    currentCameraPosition = cameraState.position4;
                }
                Debug.Log("State 3");
                break;
            case cameraState.position4:
                if (left)
                {
                    toPosition = cameraPosition3.transform.position;
                    toRotation = cameraPosition3.transform.rotation;
                    currentCameraPosition = cameraState.position3;
                }

                if (right)
                {
                    toPosition = cameraPosition1.transform.position;
                    toRotation = cameraPosition1.transform.rotation;
                    currentCameraPosition = cameraState.position1;
                }
                Debug.Log("State 4");
                break;
            default:
                Debug.Log("Camera is in a weird rotation");
                break;
        }
        // Offset the follow camera
        offset = toPosition - target.position;

        // Displace the movement input to new camera position
        changeMovementDisplacement.CameraAngle(currentCameraPosition);

        // Rotate the camera
        StartCoroutine(MoveCamera(transform.position, toPosition, transform.rotation, toRotation, speedToRotateCamera));
    }

    IEnumerator MoveCamera(Vector3 fromPosition, Vector3 toPosition, Quaternion fromRotation, Quaternion toRotation, float duration)
    {
        //create float to store the time this coroutine is operating
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(fromPosition, toPosition, (elapsedTime / duration));
            transform.rotation = Quaternion.Slerp(fromRotation, toRotation, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;

            yield return 0;
        }
    }
}

