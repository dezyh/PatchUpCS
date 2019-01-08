using System;
using System.Collections.Generic;
using System.Text;
using Patchup.Diff.Data;

namespace Patchup
{
    public static class Packing
    {
        private static class Header
        {
            internal static int Format = 0;
            internal static int Deletions = 1;
            internal static int Insertions = 2;
            internal static int Replacements = 3;
            internal static int Size = 4;
        }

        private static class Deletion
        {
            internal static int Size = 1;
            internal static int Start = Header.Size;
        }

        private static class Insertion
        {
            internal static int Size = 2;
            internal static int Start;

            // Index of value and position of a single insertion
            internal static int Value = 0;
            internal static int Position = 1;
        }

        private static class Replacement
        {
            internal static int Size = 2;
            internal static int Start;

            // Index of value and position of a single insertion
            internal static int Value = 0;
            internal static int Position = 1;
        }

        /// <summary>
        /// Packs an edit script into a byte array representation for writing to disk.
        /// </summary>
        /// <param name="editScript">The edit script to pack.</param>
        /// <returns>A byte array representing the edit script.</returns>
        public static byte[] Pack(EditScript editScript)
        {
            if (editScript.Grouped)
            {
                // Count the number of edits
                int deletions = editScript.Deletions.Count;
                int insertions = editScript.Insertions.Count;
                int replacements = editScript.Replacements.Count;

                // Set the start indexes of the sections of the edit script
                Deletion.Start = Header.Size;
                Insertion.Start = Deletion.Start + deletions;
                Replacement.Start = Insertion.Start + insertions;

                // Construct an empty byte array with fixed length to pack the edit script into
                byte[] packedEditScript = new byte[Header.Size + Deletion.Size * deletions + Insertion.Size * insertions + Replacement.Size + replacements];

                // Set the header values of the byte array
                packedEditScript[Header.Format] = (byte)EditScriptFormat.Grouped;
                packedEditScript[Header.Deletions] = (byte)deletions;
                packedEditScript[Header.Insertions] = (byte)insertions;
                packedEditScript[Header.Replacements] = (byte)replacements;

                // Pack each deletion into the byte array
                for (int i = 0; i < deletions; i++)
                {
                    var deletion = editScript.Deletions[i];
                    packedEditScript[Deletion.Start + i] = (byte)deletion.Position;
                }

                // Pack each insertion into the byte array
                for (int i = 0; i < insertions; i++)
                {
                    var insertion = editScript.Insertions[i];
                    packedEditScript[Insertion.Start + 2 * i + Insertion.Value] = insertion.Value;
                    packedEditScript[Insertion.Start + 2 * i + Insertion.Position] = (byte)insertion.Position;
                }

                // Pack each replacement into the byte array
                for (int i = 0; i < replacements; i++)
                {
                    var replacement = editScript.Replacements[i];
                    packedEditScript[Replacement.Start + 2 * i + Replacement.Value] = replacement.Value;
                    packedEditScript[Replacement.Start + 2 * i + Replacement.Position] = (byte)replacement.Position;
                }

                return packedEditScript;
            }

            return null;
        }

        /// <summary>
        /// Unpacks a byte array into an edit script for patching data. 
        /// </summary>
        /// <param name="packedEditScript">The byte array representing the edit script.</param>
        /// <returns>The unpacked edit script.</returns>
        public static EditScript Unpack(byte[] packedEditScript)
        {
            EditScript editScript = new EditScript();

            // Read the format which determines how the packed edit script is read
            var format = (EditScriptFormat)packedEditScript[Header.Format];
            if (format == EditScriptFormat.Grouped)
            {
                // Read the number of deletions, insertions and replacements
                int deletions = packedEditScript[Header.Deletions];
                int insertions = packedEditScript[Header.Insertions];
                int replacements = packedEditScript[Header.Replacements];

                // Set the start indexes of the deletion, insertion and replacement sections
                Deletion.Start = Header.Size;
                Insertion.Start = Deletion.Start + deletions;
                Replacement.Start = Insertion.Start + insertions;

                // Read and all all deletions
                for (int current = 0; current < deletions; current++)
                {
                    // Read and create the deletion
                    int position = packedEditScript[Deletion.Start + current];
                    var deletion = new Diff.Data.Deletion(position);

                    // Add the deletion into the edit script
                    editScript.Deletions.Add(deletion);
                    editScript.Edits.Add(deletion);
                }

                // Read and add all insertions
                for (int current = 0; current < insertions; current++)
                {
                    // Read and create the insertion
                    byte value = packedEditScript[Insertion.Start + Insertion.Size * current + Insertion.Value];
                    int position = packedEditScript[Insertion.Start + Insertion.Size * current + Insertion.Position];
                    var insertion = new Diff.Data.Insertion(position, value);

                    // Add the insertion into the edit script
                    editScript.Insertions.Add(insertion);
                    editScript.Edits.Add(insertion);
                }

                // Read and add all replacements
                for (int current = 0; current < replacements; current++)
                {
                    // Read and create the replacement
                    byte value = packedEditScript[Replacement.Start + Replacement.Size * current + Replacement.Value];
                    int position = packedEditScript[Replacement.Start + Replacement.Size * current + Replacement.Position];
                    var insertion = new Diff.Data.Replacement(position, value);

                    // Add the replacement into the edit script
                    editScript.Insertions.Add(insertion);
                    editScript.Edits.Add(insertion);
                }
            }

            return editScript;
        }
    }
}
