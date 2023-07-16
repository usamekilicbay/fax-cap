namespace Assets.Scripts.Common.Types
{
    [System.Flags]
    public enum MovementDirection
    {
        None = 0,       // Represents no movement direction
        Left = 1,       // Binary: 0001
        Right = 2,      // Binary: 0010
        Top = 4,        // Binary: 0100
        Bottom = 8      // Binary: 1000
    }
}
