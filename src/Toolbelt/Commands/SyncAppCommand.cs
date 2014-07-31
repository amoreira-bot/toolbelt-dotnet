using Vtex.Toolbelt.CommandFramework;
using Vtex.Toolbelt.Services;

namespace Vtex.Toolbelt.Commands
{
    [Description("watch and synchronize folder with sandbox")]
    public class SyncAppCommand : SyncCommand
    {
        private readonly WatcherFactory _factory;

        public SyncAppCommand(IConsole console, IFileSystem fileSystem, LoginService login, WatcherFactory factory)
            : base(console, fileSystem, login)
        {
            _factory = factory;
        }

        protected override Watcher GetWatcher(string appName, string authenticationToken)
        {
            return _factory.CreateAppWatcher(appName, authenticationToken);
        }

        protected override void NotifyStart(string appName)
        {
            Console.WriteLine("Using app [#magenta {0}]", appName);
            base.NotifyStart(appName);
        }
    }
}
