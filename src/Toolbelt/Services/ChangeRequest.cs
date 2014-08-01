using Vtex.Toolbelt.Model;

namespace Vtex.Toolbelt.Services
{
    public class ChangeRequest
    {
        public string Action { get; set; }
        public string Path { get; set; }
        public string Content { get; set; }
        public string Encoding { get; set; }

        public static ChangeRequest FromChange(FinalizedChange change)
        {
            return new ChangeRequest
            {
                Action = change.Action.ToString().ToLower(),
                Path = change.Path,
                Encoding = change.Action == ChangeAction.Delete ? null : change.Encoding.ToString().ToLower(),
                Content = change.Content
            };
        }
    }
}