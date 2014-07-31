using System.Collections.Generic;
using Vtex.Toolbelt.CommandFramework;
using Vtex.Toolbelt.Model;
using Vtex.Toolbelt.Services;

namespace Vtex.Toolbelt.Commands
{
    public abstract class SyncCommand : Command
    {
        private readonly IFileSystem _fileSystem;
        private readonly LoginService _login;

        protected SyncCommand(IConsole console, IFileSystem fileSystem, LoginService login)
            : base(console)
        {
            _fileSystem = fileSystem;
            _login = login;
        }

        protected sealed override void InnerExecute()
        {
            var package = PackageJson.In(_fileSystem);
            package.Validate();

            var credential = _login.GetValidCredential();
            var watcher = GetWatcher(package.Name, credential.Token);
            watcher.ChangesSent += ChangesSent;

            Console.WriteLine();
            NotifyStart(package.Name);

            Console.WriteLine();
            Console.WriteLine("Uploading current state...");
            watcher.Resync();
            Console.WriteLine("Done!");

            Console.WriteLine();
            Console.WriteLine("Waiting for changes...");
            watcher.Start();

            while (true) {}
        }

        protected abstract Watcher GetWatcher(string packageName, string authenticationToken);

        protected virtual void NotifyStart(string packageName)
        {
            Console.WriteLine("Starting sync...");
        }

        private void ChangesSent(IEnumerable<Change> changes, bool resync)
        {
            foreach (var change in changes)
            {
                Console.WriteLine("[#{0} {1}] {2}", change.Action == ChangeAction.Update ? "cyan" : "red",
                    change.Action.ToString().ToUpper(), change.Path);
            }

            if (!resync)
            {
                Console.WriteLine("Waiting for changes...");
            }
        }
    }
}
