using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceUIOnObject : MonoBehaviour
{

    public void ToggleTargetUI(bool targeting, GameObject objectPosition)
    {
        if (targeting)
        {
            positionOfTarget = objectPosition;
            objectToDisplay.SetActive(true);
        }
        else
            objectToDisplay.SetActive(false);
    }
    #region Variables
    public Canvas completeViewportCanvas;           // This needs to be the complete screen
    public GameObject objectToDisplay;
    public Camera cameraToShowTargetFrom;

    //Internal variables
    RectTransform canvas;
    GameObject positionOfTarget;
    #endregion

    // Use this for initialization
    void Start()
    {
        canvas = completeViewportCanvas.GetComponent<RectTransform>();
        objectToDisplay.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (positionOfTarget != null)
            objectToDisplay.transform.position = WorldToCanvasPosition(positionOfTarget.transform.position);
    }

    Vector3 WorldToCanvasPosition(Vector3 positionOfTarget)
    {
        //Vector position (percentage from 0 to 1) considering camera size.
        //For example (0,0) is lower left, middle is (0.5,0.5)
        Vector2 positionOnCanvasFromWorldSpace = cameraToShowTargetFrom.WorldToViewportPoint(positionOfTarget);

        //Calculate position considering our percentage, using our canvas size
        //So if canvas size is (1100,500), and percentage is (0.5,0.5), current value will be (550,250)
        positionOnCanvasFromWorldSpace.x *= canvas.sizeDelta.x;
        positionOnCanvasFromWorldSpace.y *= canvas.sizeDelta.y;

        return positionOnCanvasFromWorldSpace;
    }
}
