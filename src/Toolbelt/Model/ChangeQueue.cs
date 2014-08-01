using System.Collections.Generic;
using System.IO;
using System.Linq;
using Vtex.Toolbelt.Services;

namespace Vtex.Toolbelt.Model
{
    public class ChangeQueue
    {
        private readonly Queue<Change> _queue = new Queue<Change>(); 
        private readonly IFileSystem _fileSystem;

        public ChangeQueue(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public ChangeQueue(IEnumerable<Change> changes, IFileSystem fileSystem)
            : this(fileSystem)
        {
            _queue = new Queue<Change>(changes);
        }

        public void Enqueue(Change change)
        {
            lock(_queue)
                _queue.Enqueue(change);
        }

        public Change Dequeue(Change change)
        {
            lock (_queue)
                return _queue.Dequeue();
        }

        public IEnumerable<FinalizedChange> Summarize()
        {
            lock (_queue)
            {
                var changes = _queue.Reverse();

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
                _queue.Clear();
                return updatedPaths.Select(FinalizeUpdate).Union(deletedPaths.Select(FinalizeDeletion));
            }
        }

        public Change[] ToArray()
        {
            return _queue.ToArray();
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

        protected virtual FinalizedChange FinalizeUpdate(string fullPath)
        {
            var path = _fileSystem.GetRelativePath(fullPath);
            return _fileSystem.IsBinary(path)
                ? FinalizedChange.ForUpdate(path, _fileSystem.ReadBytes(path))
                : FinalizedChange.ForUpdate(path, _fileSystem.ReadNormalizedText(path));
        }

        protected virtual FinalizedChange FinalizeDeletion(string fullPath)
        {
            var path = _fileSystem.GetRelativePath(fullPath);
            return FinalizedChange.ForDeletion(path);
        }
    }
}