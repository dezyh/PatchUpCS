namespace Patchup.Diff.Data
{
    public enum Direction
    {
        Forward, Reverse
    }

    public static class DirectionModifer
    {
        public static Direction Opposite(Direction direction)
        {
            if (direction.Equals(Direction.Forward)) 
            {
                return Direction.Reverse;
            }
            else
            {
                return Direction.Forward;
            }
        }
    }
}
