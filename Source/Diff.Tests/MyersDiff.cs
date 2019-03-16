using Xunit;
using System.Drawing;
using Patchup.Diff.Data;
using static Patchup.Diff.Data.Direction;

namespace Patchup.Tests.Diff.Logic
{
    public class Snake
    {
        [Theory]
        // Generic Case 1 (F/R)
        [InlineData("testing", "rested", "0,0,6,7", 1, 1, Forward, 3)]
        //[InlineData("testing", "rested", "0,0,6,7", 2, 3, Reverse, 3)]
        // Generic Case 2
        [InlineData("testing", "rested", "0,0,6,7", 3, 0, Forward, 2)]
        //[InlineData("testing", "rested", "0,0,6,7", 1, 5, Reverse, 2)]
        // Generic Case 3
        [InlineData("testing", "rested", "0,0,6,7", 0, 0, Forward, 0)]
        //[InlineData("testing", "rested", "0,0,6,7", 5, 6, Reverse, 0)]
        // Other
        [InlineData("cba", "cbae", "1,1,3,3", 0, 0, Reverse, 2)]
        [InlineData("cba", "cbae", "1,1,4,3", 1, 0, Reverse, 2)]
        public void Length(string original, string patched, string boundsText, int x, int y, Direction direction, int expected)
        {
            Bounds bounds = null;
            if (boundsText != null)
            {
                string[] bound = boundsText.Split(",");
                bounds = new Bounds(new Point(int.Parse(bound[0]), int.Parse(bound[1])), new Point(int.Parse(bound[2]), int.Parse(bound[3])));
            }
            Assert.Equal(expected, Patchup.Diff.Logic.Snake.Length(original, patched, bounds, x, y, direction));
        }
    }
}

