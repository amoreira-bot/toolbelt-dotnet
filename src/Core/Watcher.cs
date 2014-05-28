using System;
using System.IO;

namespace Vtex.Toolbelt.Core
{
    public class Watcher
    {
        private readonly string accountName;
        private readonly string sessionName;
        private readonly string rootPath;

        private FileSystemWatcher fileSystemWatcher;

        public Watcher(string accountName, string sessionName, string rootPath)
        {
            this.accountName = accountName;
            this.sessionName = sessionName;
            this.rootPath = rootPath;
        }

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
            var watcher = new FileSystemWatcher(Path.Combine(this.rootPath, accountName));
            watcher.IncludeSubdirectories = true;

            watcher.Created += (sender, args) => this.OnCreated(args.FullPath);
            watcher.Changed += (sender, args) => this.OnChanged(args.FullPath);
            watcher.Deleted += (sender, args) => this.OnDeleted(args.FullPath);
            watcher.Renamed += (sender, args) =>
            {
                this.OnDeleted(args.OldFullPath);
                this.OnCreated(args.FullPath);
            };

            watcher.Error += (sender, args) => this.OnError(args.GetException());
            return watcher;
        }

        protected void OnCreated(string path)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("CREATE ");
            Console.ResetColor();
            Console.WriteLine(path.Substring(rootPath.Length));
        }

        protected void OnChanged(string path)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("UPDATE ");
            Console.ResetColor();
            Console.WriteLine(path.Substring(rootPath.Length));
        }

        protected void OnDeleted(string path)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("DELETE ");
            Console.ResetColor();
            Console.WriteLine(path.Substring(rootPath.Length));
        }

        protected void OnError(Exception exception)
        {
            Console.Error.WriteLine(exception);
        }
    }
}
