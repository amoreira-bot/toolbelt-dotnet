using System.Collections.Generic;
using Vtex.Toolbelt.Model;

namespace Vtex.Toolbelt.Services
{
    public class AccountWatcher : Watcher
    {
        private readonly string _accountName;
        private readonly string _workspace;
        private readonly GalleryClient _galleryClient;

        public AccountWatcher(string accountName, string workspace, string rootPath, string authenticationToken,
            Configuration configuration)
            : base(rootPath, configuration)
        {
            _accountName = accountName;
            _workspace = workspace;
            _galleryClient = new GalleryClient(rootPath, authenticationToken, configuration.GalleryEndpoint);
        }

        protected override void SendChanges(IList<Change> changes, bool resync)
        {
            _galleryClient.SendWorkspaceChanges(_accountName, _workspace, changes, resync);
            base.SendChanges(changes, resync);
        }
    }
}