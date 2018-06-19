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

    public enum CameraPositionChangeState
    {
        Ready,
        Changing,
        Verifying
    }

    public enum CameraPositionStates
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

    public enum WorldUiState
    {
        NoUi,
        Poi,
        InfoPrompt,
        InteractionPrompt,
        DeactivateUi
    }
}
