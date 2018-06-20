using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CloudsOfAvarice
{

    public class PlayerController : MonoBehaviour, IController3D
    {

        #region IController3D interface
        public bool Attack()
        {
            return Input.GetButtonDown(Inputs.ATTACK);
        }

        public bool Dodge()
        {
            return Input.GetButtonDown(Inputs.DASH);
        }

        public bool EndJump()
        {
            return Input.GetButtonUp(Inputs.JUMP);
        }

        public bool Jump()
        {
            return Input.GetButtonDown(Inputs.JUMP);
        }

        public float MoveHorizontal()
        {
            return Input.GetAxisRaw(Inputs.HORIZONTALMOVEMENT);
        }

        public float MoveVertical()
        {
            return Input.GetAxisRaw(Inputs.VERTICALMOVEMENT);
        }

        public bool Sprint()
        {
            return Input.GetButton(Inputs.SPRINT);
        }

        public bool Interact()
        {
            return Input.GetButtonUp(Inputs.JUMP);
        }
        #endregion

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}