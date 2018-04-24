using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActionState
{

    bool Dashing { get; set; }
    bool Jumping { get; set; }
    bool Idle { get; set; }
    bool Moving { get; set; }
    bool Sprinting { get; set; }
    bool ReversingTime { get; set; }
    bool Crouching { get; set; }
}
