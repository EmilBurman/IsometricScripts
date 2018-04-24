using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IController3D {

    //Start interface
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
        return Input.GetAxisRaw(Inputs.HORIZONTAL);
    }

    public float MoveVertical()
    {
        return Input.GetAxisRaw(Inputs.VERTICAL);
    }

    public bool Sprint()
    {
        return Input.GetButton(Inputs.SPRINT);
    }
    //End interface

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
