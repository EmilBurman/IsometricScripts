using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IJump
{
    void SetContinousJump(bool continuousJump, bool endJump);
    void Grounded(bool jump, bool sprint);
    void Airborne(bool jump);
}
