using System;

namespace Patchup.Diff.Data
{
    public class EditDistanceArray
    {
        // Arrays to store the x-distances along k-lines in both forward and reverse directions.
        public int?[] Forward { get; set; }
        public int?[] Reverse { get; set; }

        // The size difference between the two byte arrays.
        public int Delta { get; set; }
        // The minimum x-distance required for an overlap on two corresponding k-lines. 
        public int Overlap { get; set; }
        // The difference between forward and reverse k-line indexes to determine corresponding k-lines.
        public int Offset { get; set; }

        public EditDistanceArray(int originalLength, int patchedLength)
        {
            this.Forward = new int?[originalLength + patchedLength + 1];
            this.Reverse = new int?[originalLength + patchedLength + 1];
            this.Offset = originalLength;
            this.Delta = originalLength - patchedLength;
            this.Overlap = patchedLength;
            this.SetX(1, Direction.Forward, 0);
            this.SetX(1, Direction.Reverse, 0);
        }

        public int? GetY(int k, Direction direction)
        {
            if (direction == Direction.Forward)
            {
                return this.Forward[k + Offset] - k;
            }
            else
            {
                return this.Reverse[-k + Offset - Delta] - k;
            }
        }

        public int? GetX(int k, Direction direction)
        {
            if (direction == Direction.Forward)
            {
                return this.Forward[k + Offset];
            }
            else
            {
                return this.Reverse[-k + Offset - Delta];
            }
        }

        public void SetX(int k, Direction direction, int x)
        {
            if (direction == Direction.Forward)
            {
                int index = k + this.Offset;
                if (index >= 0)
                {
                    this.Forward[index] = x;
                }
            }
            else
            {
                int index = -k + this.Offset - this.Delta; 
                if (index >= 0)
                {
                    this.Reverse[index] = x;
                }
            }
        }

        public bool Overlapped(int k, Direction direction)
        {
            // If given in reverse, convert k to forwards direction
            if (direction == Direction.Reverse)
            {
                k = -k - this.Delta;
            }

            int index = k + this.Offset;

            if (this.Forward[index] != null && this.Reverse[index] != null && this.Forward[index] + this.Reverse[index] >= this.Overlap) 
            {
                return true;
            } 
            else
            {
                return false;
            }
        }

        public void Print()
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            for (int i = 0; i < Forward.Length; i++)
            {
                Console.Write(String.Format("{0,3}", i - Offset));
            }
            Console.WriteLine();
            foreach (int? x in this.Forward)
            {
                if (x == null)
                {
                    Console.Write($"  -");
                }
                else
                {
                    Console.Write($"  {x}");
                }
            }
            Console.WriteLine();
            foreach (int? x in this.Reverse)
            {
                if (x == null)
                {
                    Console.Write($"  -");
                }
                else
                {
                    Console.Write($"  {x}");
                }
            }
            //-k + Offset - Delta
            Console.WriteLine();
            for (int i = 0; i < Forward.Length; i++)
            {
                Console.Write(String.Format("{0,3}", -i + Offset - Delta));
            }
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
