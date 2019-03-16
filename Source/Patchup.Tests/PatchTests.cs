using System;
using System.Collections.Generic;
using System.Text;

using Xunit;
using Patchup;
using Patchup.Diff.Data;

namespace Patchup.Tests
{
    public class PatchTests
    {
        [Fact]
        public void Apply_Insertion()
        {
            string original_text = "hello";
            string expected_text = "ohello";

            // Convert 
            byte[] original_bytes = Encoding.UTF8.GetBytes(original_text);

            EditScript script = new EditScript();
            script.Add(new Insertion(0, 111));

            Patch patch = new Patch {EditScript = script};

            byte[] patched_data = patch.Apply(original_bytes);
            string patched_text = Encoding.UTF8.GetString(patched_data);

            Assert.Equal(expected_text, patched_text);
        }

        [Fact]
        public void Apply_Deletion()
        {
            string original_text = "hello";
            string expected_text = "hllo";

            // Convert 
            byte[] original_bytes = Encoding.UTF8.GetBytes(original_text);

            EditScript script = new EditScript();
            script.Add(new Deletion(1));

            Patch patch = new Patch {EditScript = script};

            byte[] patched_data = patch.Apply(original_bytes);
            string patched_text = Encoding.UTF8.GetString(patched_data);

            Assert.Equal(expected_text, patched_text);
        }

        [Fact]
        public void Apply_Replacement()
        {
            string original_text = "hello";
            string expected_text = "bello";

            // Convert 
            byte[] original_bytes = Encoding.UTF8.GetBytes(original_text);

            EditScript script = new EditScript();
            script.Add(new Replacement(0, 98));

            Patch patch = new Patch {EditScript = script};

            byte[] patched_data = patch.Apply(original_bytes);
            string patched_text = Encoding.UTF8.GetString(patched_data);

            Assert.Equal(expected_text, patched_text);
        }

        [Fact]
        public void Apply_Sequence()
        {
            // Set original and expected
            string original_text = "hello";
            string expected_text = "eblol";

            // Encode the original UTF-8 text to byte array 
            byte[] original_bytes = Encoding.UTF8.GetBytes(original_text);

            // Create the edit script to patch original to expected
            EditScript script = new EditScript();
            script.Add(new Replacement(2, 98));
            script.Add(new Deletion(0));
            script.Add(new Insertion(4, 108));
            
            // Create the Patchup patch from the edit script
            Patch patch = new Patch {EditScript = script};

            // Apply the Patchup patch to the data
            byte[] patched_data = patch.Apply(original_bytes);

            // Decode the patched byte array back to UTF-8 text
            string patched_text = Encoding.UTF8.GetString(patched_data);

            Assert.Equal(expected_text, patched_text);
        }

        [Fact]
        public void Build_EmptyFromValidTo()
        {
            // Set and encode from
            string from = "";
            var from_bytes = Encoding.UTF8.GetBytes(from);

            // Set and encode to
            string to = "valid";
            var to_bytes = Encoding.UTF8.GetBytes(to);

            // Build the patch
            Patch patch = new Patch();
            patch.Build(from_bytes, to_bytes);

            // Apply the patch
            byte[] expectedPatched = to_bytes;
            byte[] actualPatched = patch.Apply(from_bytes);

            // Test for equality
            Assert.Equal(expectedPatched, actualPatched);
        }

        [Fact]
        public void Build_ValidFromEmptyTo()
        {
            // Set and encode from
            string from = "valid";
            var from_bytes = Encoding.UTF8.GetBytes(from);

            // Set and encode to
            string to = "";
            var to_bytes = Encoding.UTF8.GetBytes(to);

            // Build the patch
            Patch patch = new Patch();
            patch.Build(from_bytes, to_bytes);

            // Apply the patch
            byte[] expectedPatched = to_bytes;
            byte[] actualPatched = patch.Apply(from_bytes);

            // Test for equality
            Assert.Equal(expectedPatched, actualPatched);
        }

        [Fact]
        public void Build_FromExactlyTheSameAsTo()
        {
            // Set and encode from
            string from = "same";
            var from_bytes = Encoding.UTF8.GetBytes(from);

            // Set and encode to
            string to = "same";
            var to_bytes = Encoding.UTF8.GetBytes(to);

            // Build the patch
            Patch patch = new Patch();
            patch.Build(from_bytes, to_bytes);

            // Apply the patch
            byte[] expectedPatched = to_bytes;
            byte[] actualPatched = patch.Apply(from_bytes);

            // Test for equality
            Assert.Equal(expectedPatched, actualPatched);
        }

        [Fact]
        public void Build_FromSameLengthAsTo()
        {
            // Set and encode from
            string from = "same-";
            var from_bytes = Encoding.UTF8.GetBytes(from);

            // Set and encode to
            string to = "length";
            var to_bytes = Encoding.UTF8.GetBytes(to);

            // Build the patch
            Patch patch = new Patch();
            patch.Build(from_bytes, to_bytes);

            // Apply the patch
            byte[] expectedPatched = to_bytes;
            byte[] actualPatched = patch.Apply(from_bytes);

            // Test for equality
            Assert.Equal(expectedPatched, actualPatched);
        }

        [Fact]
        public void Build_FromLongerThanTo()
        {
            // Set and encode from
            string from = "longer";
            var from_bytes = Encoding.UTF8.GetBytes(from);

            // Set and encode to
            string to = "short";
            var to_bytes = Encoding.UTF8.GetBytes(to);

            // Build the patch
            Patch patch = new Patch();
            patch.Build(from_bytes, to_bytes);

            // Apply the patch
            byte[] expectedPatched = to_bytes;
            byte[] actualPatched = patch.Apply(from_bytes);

            // Test for equality
            Assert.Equal(expectedPatched, actualPatched);
        }

        [Fact]
        public void Build_FromShorterThanTo()
        {
            // Set and encode from
            string from = "short";
            var from_bytes = Encoding.UTF8.GetBytes(from);

            // Set and encode to
            string to = "longer";
            var to_bytes = Encoding.UTF8.GetBytes(to);

            // Build the patch
            Patch patch = new Patch();
            patch.Build(from_bytes, to_bytes);

            // Apply the patch
            byte[] expectedPatched = to_bytes;
            byte[] actualPatched = patch.Apply(from_bytes);

            // Test for equality
            Assert.Equal(expectedPatched, actualPatched);
        }

        [Fact]
        public void Build_FromNullToValid_ThrowsNullReferenceException()
        {
            // Set and encode from
            byte[] from_bytes = null;

            // Set and encode to
            string to = "valid";
            var to_bytes = Encoding.UTF8.GetBytes(to);

            // Build the patch
            Patch patch = new Patch();
            Action patching = () => patch.Build(from_bytes, to_bytes);

            //assert
            Assert.Throws<NullReferenceException>(patching);
        }

        [Fact]
        public void Build_FromValidToNull_ThrowsNullReferenceException()
        {
            // Set and encode from
            
            byte[] from_bytes = Encoding.UTF8.GetBytes("valid");
            byte[] to_bytes = null;

            // Build the patch
            Patch patch = new Patch();
            Action patching = () => patch.Build(from_bytes, to_bytes);

            //assert
            Assert.Throws<NullReferenceException>(patching);
        }

        [Fact]
        public void Build_WeirdCase1()
        {
            // Set and encode from
            string from = "i";
            var from_bytes = Encoding.UTF8.GetBytes(from);

            // Set and encode to
            string to = "joker";
            var to_bytes = Encoding.UTF8.GetBytes(to);

            // Build the patch
            Patch patch = new Patch();
            patch.Build(from_bytes, to_bytes);

            // Apply the patch
            byte[] expectedPatched = to_bytes;
            byte[] actualPatched = patch.Apply(from_bytes);

            // Test for equality
            Assert.Equal(expectedPatched, actualPatched);
        }

        [Fact]
        public void Build_WeirdCase2()
        {
            // Set and encode from
            string from = "joker";
            var from_bytes = Encoding.UTF8.GetBytes(from);

            // Set and encode to
            string to = "i";
            var to_bytes = Encoding.UTF8.GetBytes(to);

            // Build the patch
            Patch patch = new Patch();
            patch.Build(from_bytes, to_bytes);

            // Apply the patch
            byte[] expectedPatched = to_bytes;
            byte[] actualPatched = patch.Apply(from_bytes);

            // Test for equality
            Assert.Equal(expectedPatched, actualPatched);
        }
    }
}
