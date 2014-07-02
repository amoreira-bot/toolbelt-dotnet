using System.Collections.Generic;
using Vtex.Toolbelt.Core;

namespace Vtex.Toolbelt.Tests
{
    public class TestableWatcher : Watcher
    {
        private readonly Dictionary<string, string[]> folders = new Dictionary<string, string[]>();
        public List<string> UpdatedPaths { get; set; }
        public List<string> DeletedPaths { get; set; }
        public int DebounceCount { get; set; }

        public new Change[] Changes
        {
            get { return base.Changes.ToArray(); }
        }

        public TestableWatcher()
            : base(null, null, null, new Configuration())
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

        public void PubliclyUpdatePath(string path)
        {
            base.UpdatePath(path);
        }

        public void PubliclyDeletePath(string path)
        {
            base.DeletePath(path);
        }

        protected override void UpdatePath(string path)
        {
            this.UpdatedPaths.Add(path);
        }

        protected override void DeletePath(string path)
        {
            this.DeletedPaths.Add(path);
        }

        protected override void Debounce()
        {
            this.DebounceCount++;
        }

        protected override bool IsFolder(string path)
        {
            return this.folders.ContainsKey(path);
        }

        protected override IEnumerable<string> ListFilesInFolder(string folderPath)
        {
            return this.folders[folderPath];
        }

        public void AddFolder(string folderPath, string[] filePaths)
        {
            this.folders.Add(folderPath, filePaths);
        }
    }
}