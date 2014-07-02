namespace Vtex.Toolbelt.Core
{
    public class ChangeBatchRequest
    {
        public string AccountName { get; set; }
        public string Workspace { get; set; }
        public string UserCookie { get; set; }
        public ChangeRequest[] Changes { get; set; }
    }
}