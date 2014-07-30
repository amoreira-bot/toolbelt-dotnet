using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Vtex.Toolbelt.Model
{
    public class ChangeQueue : Queue<Change>
    {
        public ChangeQueue()
        {
        }

        public ChangeQueue(IEnumerable<Change> changes) : base(changes)
        {
        }

        public IEnumerable<Change> Summarize(string rootPath)
        {
            var changes = this.Reverse();
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
            return updatedPaths.Select(path => new Change(ChangeAction.Update, NormalizePath(path, rootPath)))
                .Union(deletedPaths.Select(path => new Change(ChangeAction.Delete, NormalizePath(path, rootPath))));
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

        protected virtual string NormalizePath(string path, string rootPath)
        {
            return path.Substring(rootPath.Length + 1).Replace('\\', '/');
        }
    }
}