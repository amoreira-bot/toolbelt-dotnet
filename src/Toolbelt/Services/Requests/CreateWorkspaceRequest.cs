namespace Vtex.Toolbelt.Services.Requests
{
    public class CreateWorkspaceRequest
    {
        public string Name { get; set; }

        public CreateWorkspaceRequest()
        {
        }

        public CreateWorkspaceRequest(string name)
        {
            Name = name;
        }
    }
}
