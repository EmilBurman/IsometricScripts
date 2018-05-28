namespace StateEnumerators
{
    public enum DodgeState
    {
        Ready,
        Dodging,
        Cooldown
    }

    public enum AttackState
    {
        Ready,
        Attacking,
        Cooldown
    }

    public enum TimeState
    {
        Ready,
        Reversing,
        Cooldown
    }

    public enum Directions
    {
        Up,
        Down,
        Left,
        Right,
        DownLeft,
        DownRight,
        UpRight,
        UpLeft,
        Unknown
    }

    public enum cameraState
    {
        Following,
        Targeting,
        Sprinting,
        Interacting
    }

    public enum PatrolState
    {
        PatrolRight,
        PatrolLeft,
        Stop
    }

    public enum TargetState
    {
        NoTarget,
        Targeting
    }
}
