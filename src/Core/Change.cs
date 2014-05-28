namespace Vtex.Toolbelt.Core
{
    public class Change
    {
        public ChangeAction Action { get; set; }
        public string Path { get; set; }

        public Change(ChangeAction action, string path)
        {
            this.Action = action;
            this.Path = path;
        }
    }

    public enum ChangeAction
    {
        Update,
        Delete
    }
}