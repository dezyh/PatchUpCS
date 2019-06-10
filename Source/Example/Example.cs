using System;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Patchup;

namespace Patchup
{
    class Example
    {
        static void Main(string[] args)
        {
            var source = File.ReadAllBytes(@"C:\Coding\Resources\Patchup\s1.jpg");
            var target = File.ReadAllBytes(@"C:\Coding\Resources\Patchup\s2.jpg");

            //var source = Encoding.UTF8.GetBytes("source text");
            //var target = Encoding.UTF8.GetBytes("target text");

            Console.WriteLine(DateTime.Now);

            // Create a patch from source to target
            var patch = new Patch(source, target);

            Console.WriteLine(DateTime.Now);

            // Save the patch to a .patch file
            //patch.Save("t");

            // Load a patch from a .patch file
            //patch.Load("example");

            // Apply a patch to the source to get the target
            var patched = patch.Apply(source);

            //Console.WriteLine(Encoding.UTF8.GetChars(patched));
            File.WriteAllBytes(@"C:\Coding\Resources\Patchup\s3.jpg", patched);


            Console.WriteLine("Done");

            Console.ReadKey();
        }
    }
}
