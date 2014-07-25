namespace Vtex.Toolbelt.Services
{
    public class ChangeBatchRequest
    {
        public string Message { get; set; }
        public ChangeRequest[] Changes { get; set; }
    }
}