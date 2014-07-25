using System;

namespace Vtex.Toolbelt.Model
{
    public class Change : IEquatable<Change>
    {
        public ChangeAction Action { get; set; }
        public string Path { get; set; }

        public Change(ChangeAction action, string path)
        {
            this.Action = action;
            this.Path = path;
        }

        public bool Equals(Change other)
        {
            return this.Action == other.Action && this.Path == other.Path;
        }

        public override string ToString()
        {
            return string.Join(" ", this.Action, this.Path);
        }
    }
}