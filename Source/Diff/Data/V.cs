using System;
using System.Collections.Generic;
using System.Text;
using Patchup.Diff.Data;

namespace Diff.Data
{
    public static class V
    {
        public struct Array
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

            public Array(int originalLength, int patchedLength)
            {
                this.Forward = new int?[originalLength + patchedLength + 1];
                this.Reverse = new int?[originalLength + patchedLength + 1];
                this.Offset = originalLength;
                this.Delta = originalLength - patchedLength;
                this.Overlap = patchedLength;
                SetX(this, 1, Direction.Forward, 0);
                SetX(this,1, Direction.Reverse, 0);
            }
        }

        public static int GetY(Array v, int k, Direction direction)
        {
            if (direction == Direction.Forward)
            {
                return (int)v.Forward[k + v.Offset] - k;
            }
            else
            {
                return (int)v.Reverse[-k + v.Offset - v.Delta] - k;
            }
        }

        public static int GetX(Array v, int k, Direction direction)
        {
            if (direction == Direction.Forward)
            {
                return (int)v.Forward[k + v.Offset];
            }
            else
            {
                return (int)v.Reverse[-k + v.Offset - v.Delta];
            }
        }

        public static void SetX(Array v, int k, Direction direction, int x)
        {
            if (direction == Direction.Forward)
            {
                int index = k + v.Offset;
                if (index >= 0)
                {
                    v.Forward[index] = x;
                }
            }
            else
            {
                int index = -k + v.Offset - v.Delta;
                if (index >= 0)
                {
                    v.Reverse[index] = x;
                }
            }
        }

        public static bool Overlapped(Array v, int k, Direction direction)
        {
            // If given in reverse, convert k to forwards direction
            if (direction == Direction.Reverse)
            {
                k = -k - v.Delta;
            }

            int index = k + v.Offset;

            if (v.Forward[index] != null && v.Reverse[index] != null && v.Forward[index] + v.Reverse[index] >= v.Overlap)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static void Print(Array v)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            for (int i = 0; i < v.Forward.Length; i++)
            {
                Console.Write(String.Format("{0,3}", i - v.Offset));
            }
            Console.WriteLine();
            foreach (int? x in v.Forward)
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
            foreach (int? x in v.Reverse)
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
            for (int i = 0; i < v.Forward.Length; i++)
            {
                Console.Write(String.Format("{0,3}", -i + v.Offset - v.Delta));
            }
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
