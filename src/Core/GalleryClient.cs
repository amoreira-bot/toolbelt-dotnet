using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Vtex.Toolbelt.Core
{
    public class GalleryClient
    {
        private readonly string rootPath;

        public GalleryClient(string rootPath)
        {
            this.rootPath = rootPath;
        }

        public IEnumerable<Change> SendChanges(Change[] changes)
        {
            var result = SummarizeChanges(changes).ToArray();
            return result;
        }

        protected IEnumerable<Change> SummarizeChanges(IEnumerable<Change> changes)
        {
            changes = changes.Reverse();
            var deletedPaths = new List<string>();
            var updatedPaths = new List<string>();
            foreach (var change in changes)
            {
                switch (change.Action)
                {
                    case ChangeAction.Update:
                        if (ShouldUpdate(change.Path, updatedPaths, deletedPaths))
                            updatedPaths.Add(change.Path);
                        break;

                    case ChangeAction.Delete:
                        if (ShouldDelete(change.Path, updatedPaths, deletedPaths))
                            deletedPaths.Add(change.Path);
                        break;
                }
            }
            return updatedPaths.Select(path => new Change(ChangeAction.Update, NormalizePath(path)))
                .Union(deletedPaths.Select(path => new Change(ChangeAction.Delete, NormalizePath(path))));
        }

        private static bool ShouldUpdate(string changePath, ICollection<string> updatedPaths,
            ICollection<string> deletedPaths)
        {
            return !deletedPaths.Contains(changePath)
                   && !updatedPaths.Contains(changePath)
                   && !deletedPaths.Any(deleted => changePath.StartsWith(deleted + Path.DirectorySeparatorChar));
        }

        private static bool ShouldDelete(string changePath, ICollection<string> updatedPaths,
            ICollection<string> deletedPaths)
        {
            return !updatedPaths.Contains(changePath)
                   && !deletedPaths.Contains(changePath);
        }

        protected virtual string NormalizePath(string path)
        {
            return path.Substring(this.rootPath.Length + 1).Replace('\\', '/');
        }
    }
}
