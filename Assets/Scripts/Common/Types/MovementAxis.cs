namespace Assets.Scripts.Common.Types
{
    [System.Flags]
    public enum MovementAxis
    {
        Initial = 1,    // Binary: 0001
        Neutral = 2,    // Binary: 0010
        Horizontal = 4, // Binary: 0100
        Vertical = 8    // Binary: 1000
    }
}
