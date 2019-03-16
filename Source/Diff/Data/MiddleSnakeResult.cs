using System;

namespace Patchup.Diff.Data
{
    /// <summary>
    /// Stores the result of one iteration of the recursive Myers diff algorithm. 
    /// The result consists of 1 edit corresponding to the middle snake and 2 bounds 
    /// that arise from removing the middle snake from the search space. 
    /// </summary>
    public class MiddleSnakeResult
    {
        public Edit Edit { get; set; }
        public Bounds Lower { get; set; }
        public Bounds Upper { get; set; }

        public MiddleSnakeResult(Edit edit, Bounds lower, Bounds upper)
        {
            this.Edit = edit;
            this.Lower = lower;
            this.Upper = upper;
        }

        public override string ToString()
        {
            string lower = "null";
            string upper = "null";
            string edit = "null";

            if (Edit != null)
            {
                edit = Edit.ToString();
            }
            if (Lower != null)
            {
                lower = Lower.ToString();
            }
            if (Upper != null)
            {
                upper = Upper.ToString();
            }

            return $"{lower} | {edit} | {upper}";
        }
    }
}
