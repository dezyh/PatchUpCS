using Patchup.Diff.Data;
using System;
using System.Text;
using System.Drawing;

namespace Patchup.Diff.Logic
{
    public static class Snake
    {
        public static int Length(byte[] original, byte[] target, Bounds bounds, int x, int y, Direction direction)
        {
            // Check if operating in reverse and convert the reverse (x,y) coordinates to forward (x,y) coordinates. 
            if (direction == Direction.Reverse)
            {
                x = bounds.Upper.X - bounds.Lower.X - x;
                y = bounds.Upper.Y - bounds.Lower.Y - y;
            }

            // Convert the local forward (x,y) coordinates (local as from the start of the bounded region) to global forward (x,y) coordinates (global as from the start of the array).
            x += bounds.Lower.X;
            y += bounds.Lower.Y;

            // Calculate the max snake length to check for, by checking how many moves possible until hitting the upper bounds (operating forwards) or lower bounds (operating in reverse)
            int snakeMax = direction == Direction.Forward ? Math.Min(bounds.Upper.X - x, bounds.Upper.Y - y) : Math.Min(x - bounds.Lower.X, y - bounds.Lower.Y);

            // If checking in the forward direction, move to the next (x,y) position.
            if (direction == Direction.Forward)
            {
                x++;
                y++;
            }

            // Check if there is a snake at each (x,y) position until the calculated max number of snakes has been reached.
            int snake = 0;
            for (snake = 0; snake < snakeMax; snake++)
            {
                // Check if there is a match at the position (x,y).
                if (target[x-1] == original[y-1])
                {
                    if (direction == Direction.Forward)
                    {
                        x++;
                        y++;
                    }
                    else
                    {
                        x--;
                        y--;
                    }
                }
                else
                {
                    return snake;
                }
            }
            // Snake reached the calculated max snake length.
            return snake;
        }

        public static int Length(string original, string patched, Bounds bounds, int x, int y, Direction direction)
        {
            return Length(Encoding.UTF8.GetBytes(original), Encoding.UTF8.GetBytes(patched), bounds, x, y, direction);
        }

        public static MiddleSnakeResult Middle(byte[] original, byte[] patched, Bounds bounds = null)
        {
            // Set bounds to default unless they have been specified. (-1 as 0 offset but +1 as extra row added to represent starting before the byte arays so cancel out)
            bounds = bounds ?? new Bounds(new Point(0, 0), new Point(patched.Length, original.Length));

            // Handle base cases with zero area
            if (bounds.IsVerticalBaseCase())
            {
                // As the lower and upper x values are the same, this implies the only move is down which denotes a deletion.
                // The index of the deletion corresponded to the x position at the start of the move. 
                Edit edit = new Deletion(bounds.ToIndex(), bounds.Lower.X);

                // Increment the lower y value of the bounds and check that it still has an area
                Bounds upper = new Bounds(new Point(bounds.Lower.X, bounds.Lower.Y+1), bounds.Upper);
                if (upper.Lower == upper.Upper)
                {
                    upper = null;
                }

                // Return the new middle snake
                return new MiddleSnakeResult(edit, null, upper);
            }

            if (bounds.IsHorizontalBaseCase())
            {
                // Across => Insertion
                Edit edit = new Insertion(bounds.ToIndex(), bounds.Lower.X, patched[bounds.Lower.X]);

                Bounds upper = new Bounds(new Point(bounds.Lower.X+1, bounds.Upper.Y), bounds.Upper);
                if (upper.Lower == upper.Upper)
                {
                    upper = null;
                }

                return new MiddleSnakeResult(edit, null, upper);
            }

            // Store the size of the bounds.
            int N = bounds.Upper.Y - bounds.Lower.Y;
            int M = bounds.Upper.X - bounds.Lower.X;

            // Create an array to store x-distances along k-lines
            EditDistanceArray V = new EditDistanceArray(N, M);

            // Keep checking until a match must have been found
            for (int d = 0; d <= (N + M + 1) / 2; d++)
            {
                // Run the algorithm forwards and backwards
                foreach (Direction direction in Enum.GetValues(typeof(Direction)))
                {
                    // Check and update every other k-line within the current bounds.
                    for (int k = -d; k <= d; k += 2)
                    {
                        // Make sure the k-line is valid.

                        bool valid = N < M && k <= M && (k != -d && k > -N || k == -d && k >= -N) ||
                                     N > M && k >= -N && (k != d && k < M || k == d && k <= M) ||
                                     N == M && k >= -N && k <= M;

                            /*
                        bool valid = ((N < M && k != -d && k > -N && k <= M) ||
                                      (N < M && k == -d && k >= -N && k <= M) ||
                                      (N > M && k != d && k >= -N && k < M) ||
                                      (N > M && k == d && k >= -N && k <= M) ||
                                      (N == M && k >= -N && k <= M))
                            ? true
                            : false;
                            */

                        /*
                        if (N < M && k != -d && k > -N && k <= M)
                        {
                            valid = true;
                        }

                        if (N < M && k == -d && k >= -N && k <= M)
                        {
                            valid = true;
                        }

                        if (N > M && k != d && k >= -N && k < M)
                        {
                            valid = true;
                        }

                        if (N > M && k == d && k >= -N && k <= M)
                        {
                            valid = true;
                        }

                        if (N == M && k >= -N && k <= M)
                        {
                            valid = true;
                        }
                        */

                        if (valid)
                        {
                            // Check if the x-distance is larger by moving down/up or right/left and store it
                            bool down = ((k == -d) || (k != d) && V.GetX(k - 1, direction) < V.GetX(k + 1, direction));

                            int startX;
                            int startY;

                            // Update the current k-lines new x-distance
                            if (down)
                            {
                                startX = (int)V.GetX(k + 1, direction);
                                startY = (int)V.GetY(k + 1, direction);
                                V.SetX(k, direction, startX);
                            }
                            else
                            {
                                startX = (int)V.GetX(k - 1, direction);
                                startY = (int)V.GetY(k - 1, direction);
                                V.SetX(k, direction, startX + 1);
                            }

                            int currentX = (int)V.GetX(k, direction);
                            int currentY = (int)V.GetY(k, direction);

                            int currentSnake = Snake.Length(original, patched, bounds, (int)V.GetX(k, direction), (int)V.GetY(k, direction), direction);

                            int endX = currentX + currentSnake;
                            int endY = currentY + currentSnake;

                            // Update the current k-lines x-distance with the snake length from that point
                            V.SetX(k, direction, endX);

                            int globalEndX;
                            int globalEndY;

                            if (direction == Direction.Forward)
                            {
                                globalEndX = endX + bounds.Lower.X;
                                globalEndY = endY + bounds.Lower.Y;
                            }
                            else
                            {
                                globalEndX = bounds.Upper.X - endX;
                                globalEndY = bounds.Upper.Y - endY;
                            }

                            bool specialCase = false;

                            if (direction == Direction.Forward)
                            {
                                if (globalEndX == bounds.Upper.X && globalEndY == bounds.Upper.Y)
                                {
                                    specialCase = true;
                                }
                            }
                            else
                            {
                                if (globalEndX == bounds.Lower.X && globalEndY == bounds.Lower.Y)
                                {
                                    specialCase = true;
                                }
                            }
                            // Check if the middle snake reached the end of the bounds in one movement
                            if (specialCase)
                            {
                                Edit edit = null;

                                // Handle exact matches (No differences)
                                if (startY == -1)
                                {
                                    return new MiddleSnakeResult(null, null, null);
                                }

                                if (direction == Direction.Forward)
                                {
                                    // Check if the move was down or across
                                    if (down)
                                    {
                                        edit = new Deletion(bounds.ToIndex(), bounds.Lower.X + startX);
                                    }
                                    else
                                    {
                                        edit = new Insertion(bounds.ToIndex(), bounds.Lower.X + startX, patched[bounds.Lower.X + startX]);
                                    }
                                }
                                else
                                {
                                    if (down)
                                    {
                                        edit = new Deletion(bounds.ToIndex(), bounds.Lower.Y + currentY);
                                    }
                                    else
                                    {
                                        edit = new Insertion(bounds.ToIndex(), bounds.Lower.Y + currentY, patched[bounds.Lower.Y + currentY]);
                                    }
                                }

                                return new MiddleSnakeResult(edit, null, null);
                            }

                            // Check if there is an overlap along the current k-line from each end
                            if (V.Overlapped(k, direction))
                            {
                                // Values to store when returning the result of the middle snake.
                                Bounds upper;
                                Bounds lower;

                                if (direction == Direction.Reverse)
                                {
                                    // Invert the positions as they are from the bottom right not the top left
                                    startX = M - startX;
                                    startY = N - startY;
                                    endX = M - endX;
                                    endY = N - endY;

                                    // Lower bounds are from the lower bound point to snake end point
                                    lower = new Bounds(new Point(bounds.Lower.X, bounds.Lower.Y), new Point(endX + bounds.Lower.X, endY + bounds.Lower.Y));

                                    // Upper bounds are from the finish to the end
                                    upper = new Bounds(new Point(endX + bounds.Lower.X, endY + bounds.Lower.Y), new Point(bounds.Upper.X, bounds.Upper.Y));
                                }
                                else
                                {
                                    lower = new Bounds(new Point(bounds.Lower.X, bounds.Lower.Y), new Point(endX + bounds.Lower.X, endY + bounds.Lower.Y));

                                    upper = new Bounds(new Point(endX + bounds.Lower.X, endY + bounds.Lower.Y), new Point(bounds.Upper.X, bounds.Upper.Y));
                                }

                                if (lower.Lower.Equals(lower.Upper))
                                {
                                    lower = null;
                                }

                                if (upper.Lower.Equals(upper.Upper))
                                {
                                    upper = null;
                                }

                                return new MiddleSnakeResult(null, lower, upper);
                            }
                        }
                    }
                }
            }
            return null;
        }

        public static MiddleSnakeResult Middle(string original, string patched, Bounds bounds = null)
        {
            return Middle(Encoding.UTF8.GetBytes(original), Encoding.UTF8.GetBytes(patched), bounds);
        }
    }
}
