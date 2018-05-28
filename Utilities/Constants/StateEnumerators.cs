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

    public enum TwoStageState
    {
        Ready,
        Action
    }

    public enum ThreeStageCooldown
    {
        Ready,
        Action,
        Cooldown
    }

    public enum CameraState
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
