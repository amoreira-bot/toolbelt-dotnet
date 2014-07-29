using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Vtex.Toolbelt.CommandFramework;
using Vtex.Toolbelt.Model;
using Vtex.Toolbelt.Services;

namespace Vtex.Toolbelt.Commands
{
    [Description("watch and synchronize folder with workspace"), Alias("sync")]
    public class SyncWorkspaceCommand : Command
    {
        private readonly IFileSystem _fileSystem;
        private readonly Configuration _configuration;
        private readonly LoginService _login;
        private readonly WatcherFactory _watcherFactory;
        private string _workspace;

        public SyncWorkspaceCommand(IConsole console, IFileSystem fileSystem, Configuration configuration,
            LoginService login, WatcherFactory watcherFactory)
            : base(console)
        {
            this._fileSystem = fileSystem;
            this._configuration = configuration;
            this._login = login;
            this._watcherFactory = watcherFactory;
            OptionSet.Add(0, "workspace", "target workspace", true, value => _workspace = value);
        }

        protected override void InnerExecute()
        {
            var package = this.ReadPackageJson();
            package.Validate();

            var credential = _login.GetValidCredential();

            Console.WriteLine("account:   " + package.Name);
            Console.WriteLine("workspace: " + _workspace);

            var watcher = _watcherFactory.CreateAccountWatcher(package.Name, _workspace, credential.Token);
            watcher.ChangesSent += ChangesSent;

            Console.WriteLine();
            Console.WriteLine("uploading current workspace state...");
            watcher.Resync();
            Console.WriteLine("done!");

            Console.WriteLine();
            Console.WriteLine("waiting for changes...");
            watcher.Start();

            while (true) ;
        }

        private PackageJson ReadPackageJson()
        {
            try
            {
                var json = _fileSystem.ReadTextFile("package.json");
                return JsonConvert.DeserializeObject<PackageJson>(json);
            }
            catch (FileNotFoundException)
            {
                throw new ApplicationException("Couldn't find package.json file.");
            }
        }

        public void ChangesSent(IEnumerable<Change> changes)
        {
            foreach (var change in changes)
            {
                Console.WriteLine("[#{0} {1}] {2}", change.Action == ChangeAction.Update ? "cyan" : "red",
                    change.Action.ToString().ToUpper(), change.Path);
            }
            Console.WriteLine("waiting for changes...");
        }
    }
}