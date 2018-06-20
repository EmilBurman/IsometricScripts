using UnityEngine;

namespace CloudsOfAvarice
{
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

    public interface IAttack
    {
        void Attack();
    }

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

    public interface IJump
    {
        void SetContinousJump(bool continuousJump, bool endJump);
        void Grounded(bool jump, bool sprint);
        void Airborne(bool jump);
    }

    public interface IMovement
    {
        void Grounded(float horizontal, float vertical);
        void ToggleTargeting(bool value, Transform target);
        void Airborne(float horizontal, float vertical);
    }

    public interface ITerrainState
    {
        bool Grounded();
        bool Airborne();
    }
}
