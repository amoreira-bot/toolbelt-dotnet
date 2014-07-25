namespace Vtex.Toolbelt.Services
{
    public class WatcherFactory
    {
        private readonly Configuration _configuration;
        private readonly IFileSystem _fileSystem;

        public WatcherFactory(Configuration configuration, IFileSystem fileSystem)
        {
            this._configuration = configuration;
            this._fileSystem = fileSystem;
        }

        public Watcher CreateAccountWatcher(string accountName, string workspace, string authenticationToken)
        {
            return new Watcher(accountName, workspace, _fileSystem.CurrentDirectory, authenticationToken,
                _configuration);
        }
    }
}
