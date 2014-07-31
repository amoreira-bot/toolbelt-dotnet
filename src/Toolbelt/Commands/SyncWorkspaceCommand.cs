using Vtex.Toolbelt.CommandFramework;
using Vtex.Toolbelt.Services;

namespace Vtex.Toolbelt.Commands
{
    [Description("watch and synchronize folder with workspace"), Alias("sync")]
    public class SyncWorkspaceCommand : SyncCommand
    {
        private readonly WatcherFactory _factory;
        private string _workspace;

        public SyncWorkspaceCommand(
            IConsole console, IFileSystem fileSystem, LoginService login, WatcherFactory factory)
            : base(console, fileSystem, login)
        {
            _factory = factory;
            OptionSet.Add(0, "workspace", "target workspace", true, value => _workspace = value);
        }

        protected override Watcher GetWatcher(string accountName, string authenticationToken)
        {
            return _factory.CreateAccountWatcher(accountName, _workspace, authenticationToken);
        }

        protected override void NotifyStart(string accountName)
        {
            Console.WriteLine("Using [#magenta {0}] of [#magenta {1}]", _workspace, accountName);
            base.NotifyStart(accountName);
        }

        protected override void BeforeSync(Watcher watcher)
        {
            Console.WriteLine("Uploading current state...");
            watcher.Resync();
            Console.WriteLine("Done!");
        }
    }
}