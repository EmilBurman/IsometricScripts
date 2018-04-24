using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceUIOnObject : MonoBehaviour
{

    public void ToggleTargetUI(bool targeting, GameObject objectPosition)
    {
        if (targeting)
        {
            positionOfTarget = objectPosition.transform.position;
            objectToDisplay.SetActive(true);
        }
        else
            objectToDisplay.SetActive(false);
    }
    #region Variables
    public Canvas parentCanvas;
    public GameObject objectToDisplay;
    public Camera cameraToTarget;

    //Internal variables
    RectTransform canvas;
    Animator anim;
    Vector3 positionOfTarget;
    #endregion

    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
        canvas = parentCanvas.GetComponent<RectTransform>();
        objectToDisplay.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (positionOfTarget != null)
            objectToDisplay.transform.position = WorldToCanvasPosition(positionOfTarget);
    }

    Vector3 WorldToCanvasPosition(Vector3 position)
    {
        //Vector position (percentage from 0 to 1) considering camera size.
        //For example (0,0) is lower left, middle is (0.5,0.5)
        Vector2 positionOnCanvasFromWorldSpace = cameraToTarget.WorldToViewportPoint(position);

        //Calculate position considering our percentage, using our canvas size
        //So if canvas size is (1100,500), and percentage is (0.5,0.5), current value will be (550,250)
        positionOnCanvasFromWorldSpace.x *= canvas.sizeDelta.x;
        positionOnCanvasFromWorldSpace.y *= canvas.sizeDelta.y;

        //The result is ready, but, this result is correct if canvas recttransform pivot is 0,0 - left lower corner.
        //But in reality its middle (0.5,0.5) by default, so we remove the amount considering cavnas rectransform pivot.
        //We could multiply with constant 0.5, but we will actually read the value, so if custom rect transform is passed(with custom pivot) , 
        //returned value will still be correct.

        positionOnCanvasFromWorldSpace.x -= canvas.sizeDelta.x * canvas.pivot.x;
        positionOnCanvasFromWorldSpace.y -= canvas.sizeDelta.y * canvas.pivot.y;

        return positionOnCanvasFromWorldSpace;
    }
}
