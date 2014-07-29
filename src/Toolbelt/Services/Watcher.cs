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
        private readonly string _rootPath;
        private readonly Debouncer _debouncer;
        private FileSystemWatcher _fileSystemWatcher;

        protected readonly ChangeQueue Changes = new ChangeQueue();

        protected Watcher(string rootPath, Configuration configuration)
        {
            _rootPath = rootPath;
            _debouncer = new Debouncer(TimeSpan.FromMilliseconds(configuration.FileSystemDelay));
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
            var changes = this.ListFilesInFolder(_rootPath).Select(path => new Change(ChangeAction.Update, path));
            var changeQueue = new ChangeQueue(changes);
            var summarizedChanges = changeQueue.Summarize(_rootPath);

            SendChanges(summarizedChanges.ToArray(), true);
        }

        public event Action<IList<Change>> ChangesSent;

        protected virtual void SendChanges(IList<Change> changes, bool resync)
        {
            ChangesSent(changes);
        }

        private FileSystemWatcher CreateFileSystemWatcher()
        {
            var watcher = new FileSystemWatcher(_rootPath)
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
                var changes = this.Changes.Summarize(_rootPath);
                this.Changes.Clear();
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