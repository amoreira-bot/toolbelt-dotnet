namespace Vtex.Toolbelt.Services
{
    public class WatcherFactory
    {
        private readonly Configuration _configuration;
        private readonly IFileSystem _fileSystem;

        public WatcherFactory(Configuration configuration, IFileSystem fileSystem)
        {
            _configuration = configuration;
            _fileSystem = fileSystem;
        }

        public AccountWatcher CreateAccountWatcher(string accountName, string workspace, string authenticationToken)
        {
            return new AccountWatcher(accountName, workspace, _fileSystem.CurrentDirectory, authenticationToken,
                _configuration);
        }

        public AppWatcher CreateAppWatcher(string appName, string authenticationToken)
        {
            return new AppWatcher(appName, _fileSystem.CurrentDirectory, authenticationToken, _configuration);
        }
    }
}
