using System;
using System.Collections.Generic;
using Vtex.Toolbelt.Core;

namespace Vtex.Toolbelt.Tests
{
    public class TestableWatcher : Watcher
    {
        private readonly Dictionary<string, string[]> folders = new Dictionary<string, string[]>();
        public List<string> UpdatedPaths { get; set; }
        public List<string> DeletedPaths { get; set; }

        public TestableWatcher()
            : base(null, null, null)
        {
            this.UpdatedPaths = new List<string>();
            this.DeletedPaths = new List<string>();
        }

        public void PubliclyOnCreated(string path)
        {
            this.OnCreated(path);
        }

        public void PubliclyOnChanged(string path)
        {
            this.OnChanged(path);
        }

        public void PubliclyOnDeleted(string path)
        {
            this.OnDeleted(path);
        }

        protected override void UpdatePath(string path)
        {
            this.UpdatedPaths.Add(path);
        }

        protected override void DeletePath(string path)
        {
            this.DeletedPaths.Add(path);
        }

        protected override bool IsFolder(string path)
        {
            return this.folders.ContainsKey(path);
        }

        protected override IEnumerable<string> ListFilesInFolder(string folderPath)
        {
            return this.folders[folderPath];
        }

        protected override void NotifyChange(string action, ConsoleColor color, string path)
        {
        }

        public void AddFolder(string folderPath, string[] filePaths)
        {
            this.folders.Add(folderPath, filePaths);
        }
    }
}