using System;

namespace Patchup.Diff.Data
{
    public enum EditType
    {
        Insertion, Deletion, Replacement
    }

    public abstract class Edit
    {
        public int Index { get; set; }
        public EditType Type { get; set; }
        public int Position { get; set; }
        public byte Value { get; set; }

        public override string ToString()
        {
            return $"[{this.Index}] {this.Type} at {this.Position} of {Convert.ToChar(this.Value)}";
        }

        public bool Equals(Edit other)
        {
            if (this.Type.Equals(other.Type) &&
                this.Position.Equals(other.Position) &&
                this.Value.Equals(other.Value))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public class Deletion : Edit
    {
        public Deletion(int position)
        {
            base.Type = EditType.Deletion;
            base.Position = position;
        }

        public Deletion(int index, int position)
        {
            base.Index = index;
            base.Type = EditType.Deletion;
            base.Position = position;
        }

        public override string ToString()
        {
            return String.Format($"[{this.Index}] {this.Type} at {this.Position}");
        }
    }

    public class Insertion : Edit
    {
        public Insertion(int position, byte value)
        {
            base.Type = EditType.Insertion;
            base.Position = position;
            base.Value = value;
        }

        public Insertion(int index, int position, byte value)
        {
            base.Index = index;
            base.Type = EditType.Insertion;
            base.Position = position;
            base.Value = value;
        }
    }

    public class Replacement : Edit
    {
        public Replacement(int position, byte value)
        {
            base.Type = EditType.Replacement;
            base.Position = position;
            base.Value = value;
        }

        public Replacement(Deletion deletion, Insertion insertion)
        {
            base.Index = Math.Min(deletion.Index, insertion.Index);
            base.Type = EditType.Replacement;
            base.Position = insertion.Position;
            base.Value = insertion.Value;
        }
    }


}
