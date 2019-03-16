using System;
using System.Drawing;

namespace Patchup.Diff.Data
{
    public class Bounds
    {
        public Point Lower { get; set; }
        public Point Upper { get; set; }

        public Bounds(Point lower, Point upper)
        {
            this.Lower = lower;
            this.Upper = upper;
        }

        public override string ToString()
        {
            return String.Format($"({this.Lower.X},{this.Lower.Y}) -> ({this.Upper.X},{this.Upper.Y})");
        }

        public int ToIndex()
        {
            return this.Lower.X + this.Lower.Y;
        }

        public bool Equals(Bounds other)
        {
            if (this.Lower.Equals(other.Lower) && 
                this.Upper.Equals(other.Upper))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int Area()
        {
            return (this.Upper.X - this.Lower.X) * (this.Upper.Y - this.Lower.Y);
        }

        public bool IsHorizontalBaseCase()
        {
            return this.Lower.Y == this.Upper.Y;
        }

        public bool IsVerticalBaseCase()
        {
            return this.Lower.X == this.Upper.X;
        }
    }
}
