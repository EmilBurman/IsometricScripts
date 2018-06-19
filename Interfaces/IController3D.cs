using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IController3D
{
    float MoveHorizontal();
    float MoveVertical();
    bool Jump();
    bool EndJump();
    bool Dodge();
    bool Sprint();
    bool Attack();
    bool Interact();
}
