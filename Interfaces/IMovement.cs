using StateEnumerators;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovement
{
    void Grounded(float horizontal, float vertical);
    void ToggleTargeting(bool value, Transform target);
    void Airborne(float horizontal, float vertical);
}
