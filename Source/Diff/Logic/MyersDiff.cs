using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Patchup.Diff.Data;

namespace Patchup.Diff.Logic
{
    public static class MyersDiff
    {
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

        public static EditScript MultiThreaded(byte[] original, byte[] target)
        {
            var script = new EditScript();
            var bounds = new BlockingCollection<Bounds>() { null };
            var remaining = bounds.Count;

            // Set the degree of parallelism to the number of processor cores
            var parallelism = new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount };
            
            // Set the partitioning to not buffer
            var partitioning = Partitioner.Create(bounds.GetConsumingEnumerable(), EnumerablePartitionerOptions.NoBuffering);

            Parallel.ForEach(partitioning, parallelism, bound =>
            {
                var result = Snake.Middle(original, target, bound);

                Console.WriteLine(result);

                if (result.Edit != null)
                {
                    script.SafeAdd(result.Edit);
                }
                if (result.Upper != null)
                {
                    Interlocked.Increment(ref remaining);
                    bounds.Add(result.Upper);
                }
                if (result.Lower != null)
                {
                    Interlocked.Increment(ref remaining);
                    bounds.Add(result.Lower);
                }

                int currentRemaining = Interlocked.Decrement(ref remaining);

                if (currentRemaining == 0)
                {
                    bounds.CompleteAdding();
                }
            });

            Console.WriteLine(DateTime.Now);

            script.Order();

            return script;
        }
    }
}
