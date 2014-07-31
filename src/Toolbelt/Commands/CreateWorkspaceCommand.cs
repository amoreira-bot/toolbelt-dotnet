using System;
using Vtex.Toolbelt.CommandFramework;
using Vtex.Toolbelt.Model;
using Vtex.Toolbelt.Services;

namespace Vtex.Toolbelt.Commands
{
    [Description("create a new workspace for an account")]
    public class CreateWorkspaceCommand : Command
    {
        private readonly IFileSystem _fileSystem;
        private readonly LoginService _login;
        private readonly Configuration _configuration;
        private string _workspace;
        private string _account;

        public CreateWorkspaceCommand(IConsole console, IFileSystem fileSystem, LoginService login,
            Configuration configuration) : base(console)
        {
            _fileSystem = fileSystem;
            _login = login;
            _configuration = configuration;
            OptionSet.Add(0, "workspace", "workspace to create", true, value => _workspace = value);
            OptionSet.Add("account", 'a', "name of account in which to create workspace", value => _account = value);
        }

        protected override void InnerExecute()
        {
            if (string.IsNullOrWhiteSpace(_account))
            {
                try
                {
                    var package = PackageJson.In(_fileSystem);
                    if(package != null)
                        _account = package.Name;
                }
                catch (ApplicationException)
                {
                }
                if (string.IsNullOrWhiteSpace(_account))
                {
                    throw new ApplicationException(
                        "Account name must be informed either in the package.json file or through the --account option");
                }
            }

            var credential = _login.GetValidCredential();
            var client = new GalleryClient("", credential.Token, _configuration.GalleryEndpoint);
            client.CreateWorkspace(_account, _workspace);

            Console.WriteLine("Workspace '{0}' created for account '{1}'", _workspace, _account);
        }
    }
}
