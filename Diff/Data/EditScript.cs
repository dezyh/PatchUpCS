using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;

namespace Patchup.Diff.Data
{
    public class EditScript : IEnumerable<Edit>
    {
        public IEnumerator<Edit> GetEnumerator()
        {
            return this.Edits.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Edits.GetEnumerator();
        }

        /// <summary>
        /// Flag which indicates whether the edit script has been grouped into insertions and deletions.
        /// </summary>
        public bool Grouped { get; set; }

        /// <summary>
        /// General list of all edits. 
        /// </summary>
        public List<Edit> Edits { get; set; }

        /// <summary>
        /// Specific list of deletions.
        /// </summary>
        public List<Edit> Deletions { get; set; }

        /// <summary>
        /// Specific list of insertions.
        /// </summary>
        public List<Edit> Insertions { get; set; }

        /// <summary>
        /// Specific list of replacements. 
        /// </summary>
        public List<Replacement> Replacements { get; set; }

        public EditScript()
        {
            this.Edits = new List<Edit>();
            this.Deletions = new List<Edit>();
            this.Insertions = new List<Edit>();
            this.Grouped = false;
        }

        /// <summary>
        /// Adds an edit into a general list of all edits.
        /// </summary>
        /// <param name="edit">The edit to add.</param>
        public void Add(Edit edit)
        {
            this.Edits.Add(edit);
        }

        /// <summary>
        /// Orders the edit script so that each edit is in the correct order in which it should have been found.
        /// This is required as when using multiple threads to calculate the edit script, edits are not guaranteed to be calculated in the correct order. 
        /// </summary>
        public void Order()
        {
            this.Edits.Sort((x,y) => x.Index.CompareTo(y.Index));
        }

        /// <summary>
        /// Reorders the edit script so that insertions, deletions and replacements can all be grouped together and in separate lists.
        /// By default, deletions are moved to the front of the edit script, followed by insertions and ending with replacements. 
        /// </summary>
        public void Group()
        {
            // Create new lists to ensure old lists are overwritten
            var insertions = new List<Edit>();
            var deletions = new List<Edit>();

            // The number of insertions seen corresponds to the decrease in the position of subsequent deletions that is required to move it before all other insertions and replacements.
            int previousInsertions = 0;

            foreach (var edit in Edits)
            {
                if (edit.Type == EditType.Insertion)
                {
                    previousInsertions++;
                    insertions.Add(edit);
                }
                else if (edit.Type == EditType.Deletion)
                {
                    edit.Position -= previousInsertions;
                    deletions.Add(edit);
                }
            }

            this.Deletions = deletions;
            this.Insertions = insertions;

            this.Grouped = true;
        }        
    }
}