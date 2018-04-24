using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerThrow : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        bool aim = Input.GetButton(Inputs.AIM);
        bool attack = Input.GetButton(Inputs.ATTACK);

    }
}
