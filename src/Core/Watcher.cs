﻿using System;
using System.Collections.Generic;
using System.IO;

namespace Vtex.Toolbelt.Core
{
    public class Watcher
    {
        private readonly string accountName;
        private readonly string sessionName;
        private readonly string rootPath;
        private readonly Debouncer debouncer;
        private readonly GalleryClient galleryClient;

        private FileSystemWatcher fileSystemWatcher;

        protected readonly List<Change> Changes = new List<Change>(); 

        public Watcher(string accountName, string sessionName, string rootPath)
        {
            this.accountName = accountName;
            this.sessionName = sessionName;
            this.rootPath = rootPath;
            this.debouncer = new Debouncer(TimeSpan.FromMilliseconds(300));
            this.galleryClient = new GalleryClient(accountName, sessionName, rootPath);
            this.galleryClient.RequestFailed += args => this.RequestFailed(args);
        }

        public event Action<IEnumerable<Change>> ChangesSent;

        public event Action<RequestFailedArgs> RequestFailed;

        public event Action<Exception> FileSystemError;

        public void Start()
        {
            if (fileSystemWatcher == null)
                fileSystemWatcher = CreateFileSystemWatcher();

            fileSystemWatcher.EnableRaisingEvents = true;
        }

        public void Stop()
        {
            if (fileSystemWatcher != null)
                fileSystemWatcher.EnableRaisingEvents = false;
        }

        private FileSystemWatcher CreateFileSystemWatcher()
        {
            var watcher = new FileSystemWatcher(this.rootPath);
            watcher.IncludeSubdirectories = true;

            watcher.Created += (sender, args) => this.OnCreated(args.FullPath);
            watcher.Changed += (sender, args) => this.OnChanged(args.FullPath);
            watcher.Deleted += (sender, args) => this.OnDeleted(args.FullPath);
            watcher.Renamed += (sender, args) =>
            {
                this.OnDeleted(args.OldFullPath);
                this.OnCreated(args.FullPath);
            };

            watcher.Error += (sender, args) => this.FileSystemError(args.GetException());
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
            this.Changes.Add(new Change(ChangeAction.Update, path));
            this.Debounce();
        }

        protected virtual void DeletePath(string path)
        {
            this.Changes.Add(new Change(ChangeAction.Delete, path));
            this.Debounce();
        }

        protected virtual void Debounce()
        {
            this.debouncer.Debounce(() =>
            {
                var sentChanges = this.galleryClient.SendChanges(this.Changes.ToArray());
                this.Changes.Clear();
                this.ChangesSent(sentChanges);
            });
        }

        protected virtual bool IsFolder(string path)
        {
            return Directory.Exists(path);
        }

        protected virtual IEnumerable<string> ListFilesInFolder(string folderPath)
        {
            return Directory.EnumerateFiles(folderPath, "*.*", SearchOption.AllDirectories);
        }
    }
}
