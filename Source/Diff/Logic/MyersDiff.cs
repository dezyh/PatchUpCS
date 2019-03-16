using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Patchup.Diff.Data;

namespace Patchup.Diff.Logic
{
    public static class MyersDiff
    {
        public static bool good = true;

        public static EditScript SingleThreaded(string original, string target)
        {
            return SingleThreaded(Encoding.UTF8.GetBytes(original), Encoding.UTF8.GetBytes(target));
        }

        public static EditScript SingleThreaded(byte[] original, byte[] target)
        {
            var script = new EditScript();

            var stack = new Stack<Bounds>();
            stack.Push(null);

            while (stack.Count > 0)
            {
                var bounds = stack.Pop();

                var result = Snake.Middle(original, target, bounds);

                if (result.Edit != null)
                {
                    script.Add(result.Edit);
                }

                // Push lower bounds after upper bounds to the stack so that lower bounds is processed first and the edit script is computed in order
                if (result.Upper != null)
                {
                    stack.Push(result.Upper);
                }
                if (result.Lower != null)
                {
                    stack.Push(result.Lower);
                }
            }

            return script;
        }

        private static IEnumerable<bool> EnumerateStackUntilEmpty<T>(ConcurrentStack<T> stack)
        {
            while (!stack.IsEmpty) yield return true;
        }

        public static EditScript MultiThreaded(byte[] original, byte[] target)
        {
            Object scriptLock = new Object();

            var script = new EditScript();
            var stack = new ConcurrentStack<Bounds>();
            stack.Push(null);

            const int MinCores = 1;
            int cores = Environment.ProcessorCount;
            var parallelOptions = new ParallelOptions() {MaxDegreeOfParallelism = cores};

            Parallel.ForEach(EnumerateStackUntilEmpty(stack), parallelOptions, item =>
            {
                if (stack.TryPop(out var bounds))
                {
                    //Console.WriteLine(bounds.ToString());

                    var result = Snake.Middle(original, target, bounds);

                    if (result.Edit != null)
                    {
                        lock (scriptLock)
                        {
                            script.Edits.Add(result.Edit);
                        }
                    }

                    if (result.Lower != null)
                    {
                        stack.Push(result.Lower);
                    }

                    if (result.Upper != null)
                    {
                        stack.Push(result.Upper);
                    }

                    if (parallelOptions.MaxDegreeOfParallelism != cores)
                    {
                        parallelOptions.MaxDegreeOfParallelism = Math.Min(cores, Math.Max(stack.Count, MinCores));
                    }
                }
                else
                {
                    
                }
            }
            );

            script.Order();

            return script;
        }
    }
}
