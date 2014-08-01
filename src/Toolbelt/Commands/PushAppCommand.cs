using Vtex.Toolbelt.CommandFramework;
using Vtex.Toolbelt.Model;
using Vtex.Toolbelt.Services;
using System;
using System.Collections.Generic;
using System.IO;

namespace Vtex.Toolbelt.Commands
{
    [Description("publish new app version"), Alias("push")]
    public class PushAppCommand : Command
    {
        private readonly LoginService _login;
        private readonly Configuration _configuration;
        private readonly IFileSystem _fileSystem;

        public PushAppCommand(IConsole console,  IFileSystem fileSystem, Configuration configuration,
            LoginService login)
            : base(console)
        {
            _configuration = configuration;
            _login = login;
            _fileSystem = fileSystem;
        }

        protected override void InnerExecute()
        {
            var credential = _login.GetValidCredential();
            var root = _fileSystem.CurrentDirectory;

            var package = PackageJson.In(_fileSystem);
            package.Validate();

            var files = this.ListFilesInFolder(root);
            var packageFiles = new PackageFiles(root);
            var compressedFiles = packageFiles.Compress(files);

            var appPublisher = new AppPublisher(credential.Token, _configuration);
            appPublisher.PublishApp(package.Name, package.Version, compressedFiles);

            Console.WriteLine("App \'{0}\' version \'{1}\' was successfully published",
                package.Name, package.Version);
        }

        private IEnumerable<string> ListFilesInFolder(string folderPath)
        {
            try
            {
                return Directory.EnumerateFiles(folderPath, "*.*", SearchOption.AllDirectories);
            }
            catch (DirectoryNotFoundException)
            {
                throw new ApplicationException(string.Format("Couldn't find path \'{0}\'", folderPath));
            }
        }
    }
}