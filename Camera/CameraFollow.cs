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
    CameraPositionStates currentCameraPosition;
    CameraPositionStates cameraState;
    public GameObject cameraPosition1, cameraPosition2, cameraPosition3, cameraPosition4;
    PlayerMovement changeMovementDisplacement;
    #endregion
    
    IEnumerator MoveCamera(Vector3 fromPosition, Vector3 toPosition, float duration)
    {
        //create float to store the time this coroutine is operating
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(fromPosition, toPosition, (elapsedTime / duration));

            elapsedTime += Time.deltaTime;

            yield return 0;
        }
    }
}

