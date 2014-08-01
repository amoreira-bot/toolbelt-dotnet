using System;

namespace Vtex.Toolbelt.Model
{
    public class FinalizedChange : IEquatable<FinalizedChange>
    {
        public ChangeAction Action { get; private set; }

        public string Path { get; private set; }

        public string Content { get; private set; }

        public ChangeEncoding Encoding { get; private set; }

        public FinalizedChange(ChangeAction action, string path)
        {
            Action = action;
            Path = path;
        }

        public static FinalizedChange ForUpdate(string path, byte[] content)
        {
            return new FinalizedChange(ChangeAction.Update, path)
            {
                Encoding = ChangeEncoding.Base64,
                Content = Convert.ToBase64String(content)
            };
        }

        public static FinalizedChange ForUpdate(string path, string text)
        {
            return new FinalizedChange(ChangeAction.Update, path)
            {
                Encoding = ChangeEncoding.Utf8,
                Content = text
            };
        }

        public static FinalizedChange ForDeletion(string path)
        {
            return new FinalizedChange(ChangeAction.Delete, path);
        }

        public bool Equals(FinalizedChange other)
        {
            return this.Action == other.Action && this.Path == other.Path;
        }

        public override string ToString()
        {
            return string.Join(" ", this.Action, this.Path);
        }
    }
}
