using System.Collections.Generic;
using Vtex.Toolbelt.Model;

namespace Vtex.Toolbelt.Services
{
    public class AppWatcher : Watcher
    {
        private readonly string _appName;
        private readonly GalleryClient _galleryClient;

        public AppWatcher(string appName, string rootPath, string authenticationToken, Configuration configuration)
            : base(rootPath, configuration)
        {
            _appName = appName;
            _galleryClient = new GalleryClient(rootPath, authenticationToken, configuration.GalleryEndpoint);
        }

        protected override void SendChanges(IList<Change> changes, bool resync)
        {
            _galleryClient.SendSandboxChanges(_appName, changes, resync);
            base.SendChanges(changes, resync);
        }
    }
}
