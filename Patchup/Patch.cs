using Patchup.Diff.Data;
using Patchup.Diff.Logic;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.IO;
using System.Net.Http.Headers;
using System.Text;
using System.Xml;
using static Patchup.Packing;

namespace Patchup
{
    public class Patch
    {
        public EditScript EditScript { get; set; }

        public Patch()
        {

        }

        public Patch(string path)
        {
            this.Load(path);
        }

        /// <summary>
        /// Patch constructors which automatically computes the edit script between the from byte array and the to byte array.
        /// </summary>
        /// <param name="from">Byte array to patch from.</param>
        /// <param name="to">Byte array to patch to.</param>
        public Patch(byte[] from, byte[] to)
        {
            this.Build(from, to);
        }

        public Patch(string from, string to)
        {
            this.Build(Encoding.UTF8.GetBytes(from), Encoding.UTF8.GetBytes(to));
        }

        /// <summary>
        /// Builds an EditScript which patches the original data to the desired data and uses this to create the Patchup patch.
        /// </summary>
        /// <param name="from">The original byte data which the patch will be applied to.</param>
        /// <param name="to">The desired byte data which will result after applying the patch to the original byte data.</param>
        public void Build(byte[] from, byte[] to)
        {
            this.EditScript = MyersDiff.SingleThreaded(from, to);
        }

        /// <summary>
        /// Applies a Patchup patch to a byte array representing some data.
        /// </summary>
        public byte[] Apply(byte[] data)
        {
            var patchedData = new List<byte>(data);

            foreach (var edit in this.EditScript)
            {
                if (edit.Type == EditType.Deletion)
                {
                    patchedData.RemoveAt(edit.Position);
                }
                else if (edit.Type == EditType.Insertion)
                {
                    patchedData.Insert(edit.Position, edit.Value);
                }
                else if (edit.Type == EditType.Replacement)
                {
                    patchedData[edit.Position] = edit.Value;
                }
            }

            return patchedData.ToArray();
        }

        public string Apply(string text)
        {
            return Encoding.UTF8.GetString(Apply(Encoding.UTF8.GetBytes(text)));
        }

        /// <summary>
        /// Saves the edit script to a Patchup (.patch) file
        /// </summary>
        public void Save(string path, bool group = true)
        {
            // If no extension supplied, prepend the default patchup file extension (.patch)
            if (!path.Contains('.'))
            {
                path += ".patchup";
            }

            // If grouping is desired and if the edit script is not already grouped, group the deletions and insertions in the edit script
            if (group && !this.EditScript.Grouped)
            {
                this.EditScript.Group();
            }

            byte[] packedEditScript = Pack(this.EditScript);

            File.WriteAllBytes(path, packedEditScript);
        }

        /// <summary>
        /// Loads an edit script from a Patchup (.patch) file
        /// </summary>
        /// <param name="path">The path to the Patchup (.patch) file.</param>
        public void Load(string path)
        {
            byte[] packedEditScript = File.ReadAllBytes(path);

            this.EditScript = Unpack(packedEditScript);
        }

        public void Print()
        {
            foreach (var edit in this.EditScript)
            {
                Console.WriteLine(edit.ToString());
            }
        }
    }
}
