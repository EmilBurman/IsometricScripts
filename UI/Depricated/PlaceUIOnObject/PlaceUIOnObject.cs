﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CloudsOfAvarice
{
    public class PlaceUIOnObject : MonoBehaviour
    {

        public void ToggleTargetUI(bool placeUIonGameObject, GameObject objectPosition = null)
        {
            if (placeUIonGameObject)
            {
                positionOfTarget = objectPosition;
                objectToDisplay.SetActive(true);
            }
            else
                objectToDisplay.SetActive(false);
        }

        #region Variables
        [Header("Setup for canvas, UI element to display and camera")]
        public Canvas completeViewportCanvas;           // This needs to be the complete screen
        public GameObject objectToDisplay;
        public Camera cameraToShowTargetFrom;

        //Internal variables
        RectTransform completeScreenRectTransform;
        GameObject positionOfTarget;
        #endregion

        // Use this for initialization
        void Start()
        {
            completeScreenRectTransform = completeViewportCanvas.GetComponent<RectTransform>();
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
            positionOnCanvasFromWorldSpace.x *= completeScreenRectTransform.sizeDelta.x;
            positionOnCanvasFromWorldSpace.y *= completeScreenRectTransform.sizeDelta.y;

            return positionOnCanvasFromWorldSpace;
        }
    }
}