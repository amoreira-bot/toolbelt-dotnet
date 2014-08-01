using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Vtex.Toolbelt.Model;

namespace Vtex.Toolbelt.Services
{
    public abstract class Watcher
    {
        private readonly IFileSystem _fileSystem;
        private readonly Debouncer _debouncer;
        private FileSystemWatcher _fileSystemWatcher;

        protected readonly ChangeQueue Changes;

        protected Watcher(IFileSystem fileSystem, Configuration configuration)
        {
            _fileSystem = fileSystem;
            _debouncer = new Debouncer(TimeSpan.FromMilliseconds(configuration.FileSystemDelay));
            Changes = new ChangeQueue(fileSystem);
        }

        public void Start()
        {
            if (_fileSystemWatcher == null)
                _fileSystemWatcher = CreateFileSystemWatcher();

            _fileSystemWatcher.EnableRaisingEvents = true;
        }

        public void Stop()
        {
            if (_fileSystemWatcher != null)
                _fileSystemWatcher.EnableRaisingEvents = false;
        }

        public void Resync()
        {
            var changes = this.ListFilesInFolder(_fileSystem.CurrentDirectory)
                .Select(path => new Change(ChangeAction.Update, path));
            var changeQueue = new ChangeQueue(changes, _fileSystem);
            var summarizedChanges = changeQueue.Summarize();

            SendChanges(summarizedChanges.ToArray(), true);
        }

        public event Action<IList<FinalizedChange>, bool> ChangesSent;

        protected virtual void SendChanges(IList<FinalizedChange> changes, bool resync)
        {
            ChangesSent(changes, resync);
        }

        private FileSystemWatcher CreateFileSystemWatcher()
        {
            var watcher = new FileSystemWatcher(_fileSystem.CurrentDirectory)
            {
                IncludeSubdirectories = true,
                NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite |
                    NotifyFilters.FileName | NotifyFilters.DirectoryName,
            };

            watcher.Created += (sender, args) => this.OnCreated(args.FullPath);
            watcher.Changed += (sender, args) => this.OnChanged(args.FullPath);
            watcher.Deleted += (sender, args) => this.OnDeleted(args.FullPath);
            watcher.Renamed += (sender, args) =>
            {
                this.OnDeleted(args.OldFullPath);
                this.OnCreated(args.FullPath);
            };

            watcher.Error += (sender, args) => { throw args.GetException(); };
            return watcher;
        }
        protected void OnCreated(string path)
        {
            if (this.IsFolder(path))
            {
                var filePaths = this.ListFilesInFolder(path);
                foreach (var file in filePaths)
                    this.UpdatePath(file);
            }
            else
            {
                this.UpdatePath(path);
            }
        }

        protected void OnChanged(string path)
        {
            if (this.IsFolder(path))
                return;

            this.UpdatePath(path);
        }

        protected void OnDeleted(string path)
        {
            this.DeletePath(path);
        }

        protected virtual void UpdatePath(string path)
        {
            this.Changes.Enqueue(new Change(ChangeAction.Update, path));
            this.Debounce();
        }

        protected virtual void DeletePath(string path)
        {
            this.Changes.Enqueue(new Change(ChangeAction.Delete, path));
            this.Debounce();
        }

        protected virtual void Debounce()
        {
            _debouncer.Debounce(() =>
            {
                var changes = this.Changes.Summarize();
                SendChanges(changes.ToArray(), false);
            });
        }

        protected virtual bool IsFolder(string path)
        {
            return Directory.Exists(path);
        }

        protected virtual IEnumerable<string> ListFilesInFolder(string folderPath)
        {
            var i = 0;
            const int maxRetries = 5;
            while (true)
            {
                i++;
                try
                {
                    return Directory.EnumerateFiles(folderPath, "*.*", SearchOption.AllDirectories);
                }
                catch
                {
                    if (i < maxRetries)
                        Task.Delay(5).Wait();
                    else
                        throw;
                }
            }
        }
    }
}