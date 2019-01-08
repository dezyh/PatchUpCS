using Patchup;

namespace Patchup
{
    class Example
    {
        static void Main(string[] args)
        {
            var source = "example text";
            var target = "target text";

            // Create a patch from source to target
            var patch = new Patch(source, target);

            // Save the patch to a .patch file
            patch.Save("example");

            // Load a patch from a .patch file
            patch.Load("example");

            // Apply a patch to the source to get the target
            var patched = patch.Apply(source);
        }
    }
}
