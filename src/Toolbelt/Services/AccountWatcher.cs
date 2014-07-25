using System;
using System.Collections.Generic;
using System.Linq;
using Vtex.Toolbelt.Model;

namespace Vtex.Toolbelt.Services
{
    public class AccountWatcher : Watcher
    {
        private readonly string accountName;
        private readonly string workspaceName;
        private readonly GalleryClient galleryClient;

        public AccountWatcher(string accountName, string workspaceName, string rootPath, string authenticationToken,
            Configuration configuration)
            : base(rootPath, configuration)
        {
            this.accountName = accountName;
            this.workspaceName = workspaceName;
            this.galleryClient = new GalleryClient(accountName, workspaceName, rootPath, authenticationToken,
                configuration.GalleryEndpoint);
        }

        public event Action<IEnumerable<Change>> ChangesSent;

        protected override void SendChanges(IEnumerable<Change> changes, bool resync)
        {
            var sentChanges = this.galleryClient.SendChanges(changes.ToArray(), resync);
            this.ChangesSent(sentChanges);
        }
    }
}