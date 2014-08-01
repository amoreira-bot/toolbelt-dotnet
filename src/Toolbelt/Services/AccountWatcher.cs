using System.Collections.Generic;
using System.IO;
using System.Linq;
using Vtex.Toolbelt.Model;
using Vtex.Toolbelt.Services.Responses;

namespace Vtex.Toolbelt.Services
{
    public class AccountWatcher : Watcher
    {
        private readonly string _accountName;
        private readonly string _workspace;
        private readonly IFileSystem _fileSystem;
        private readonly GalleryClient _galleryClient;

        public AccountWatcher(string accountName, string workspace, IFileSystem fileSystem, string authenticationToken,
            Configuration configuration)
            : base(fileSystem.CurrentDirectory, configuration)
        {
            _accountName = accountName;
            _workspace = workspace;
            _fileSystem = fileSystem;
            _galleryClient = new GalleryClient(fileSystem.CurrentDirectory, authenticationToken,
                configuration.GalleryEndpoint);
        }

        protected override void SendChanges(IList<Change> changes, bool resync)
        {
            _galleryClient.SendWorkspaceChanges(_accountName, _workspace, changes, resync);
            base.SendChanges(changes, resync);
        }

        public IEnumerable<FileConflict> IdentifyConflicts()
        {
            var remoteState = _galleryClient.GetWorkspaceState(_accountName, _workspace);
            foreach (var local in _fileSystem.GetFileStates())
            {
                FileStateResponse remote = null;
                if (remoteState.TryGetValue(local.Path, out remote))
                    remoteState.Remove(local.Path);

                if (remote == null)
                    yield return new FileConflict(local.Path, local.Size, null);

                else if (remote.Hash != local.Hash)
                    yield return new FileConflict(local.Path, local.Size, remote.Size);
            }

            foreach (var remote in remoteState)
                yield return new FileConflict(remote.Key, null, remote.Value.Size);
        }

        public void ResolveWithRemote(ICollection<FileConflict> conflicts)
        {
            foreach (var conflict in conflicts)
            {
                if (conflict.RemoteSize == null)
                {
                    _fileSystem.DeleteFile(conflict.Path);
                }
                else
                {
                    var file = _galleryClient.GetFile(_accountName, _workspace, conflict.Path);
                    _fileSystem.WriteFile(conflict.Path, file);
                }
            }
        }

        public void ResolveWithLocal(ICollection<FileConflict> conflicts)
        {
        }
    }
}