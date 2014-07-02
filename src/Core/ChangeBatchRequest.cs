namespace Vtex.Toolbelt.Core
{
    public class ChangeBatchRequest
    {
        public string Message { get; set; }
        public ChangeRequest[] Changes { get; set; }
    }
}