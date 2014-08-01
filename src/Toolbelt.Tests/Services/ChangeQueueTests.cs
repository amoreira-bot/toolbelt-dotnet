using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Vtex.Toolbelt.Model;

namespace Vtex.Toolbelt.Tests.Services
{
    [TestFixture]
    public class ChangeQueueTests
    {
        class SummarizeChanges
        {
            [Test]
            public void Update_one_file()
            {
                // Arrange
                var changes = new[]
                {
                    new Change(ChangeAction.Update, "a")
                };
                var queue = new TestableChangeQueue(changes);

                // Act
                var result = queue.Summarize();

                // Assert
                Assert.That(result, Is.EquivalentTo(new[]
                {
                    new FinalizedChange(ChangeAction.Update, "a")
                }));
            }

            [Test]
            public void Update_same_file_twice()
            {
                // Arrange
                var changes = new[]
                {
                    new Change(ChangeAction.Update, "a"),
                    new Change(ChangeAction.Update, "a")
                };
                var queue = new TestableChangeQueue(changes);

                // Act
                var result = queue.Summarize();

                // Assert
                Assert.That(result.ToArray(), Is.EquivalentTo(new[]
                {
                    new FinalizedChange(ChangeAction.Update, "a")
                }));
            }

            [Test]
            public void Delete_one_file()
            {
                // Arrange
                var changes = new[]
                {
                    new Change(ChangeAction.Delete, "a")
                };
                var queue = new TestableChangeQueue(changes);

                // Act
                var result = queue.Summarize();

                // Assert
                Assert.That(result, Is.EquivalentTo(new[]
                {
                    new FinalizedChange(ChangeAction.Delete, "a")
                }));
            }

            [Test]
            public void Delete_same_file_twice()
            {
                // Arrange
                var changes = new[]
                {
                    new Change(ChangeAction.Delete, "a"),
                    new Change(ChangeAction.Delete, "a")
                };
                var queue = new TestableChangeQueue(changes);

                // Act
                var result = queue.Summarize();

                // Assert
                Assert.That(result.ToArray(), Is.EquivalentTo(new[]
                {
                    new FinalizedChange(ChangeAction.Delete, "a")
                }));
            }

            [Test]
            public void Update_a_file_then_delete_it()
            {
                // Arrange
                var changes = new[]
                {
                    new Change(ChangeAction.Update, "a"),
                    new Change(ChangeAction.Delete, "a")
                };
                var queue = new TestableChangeQueue(changes);

                // Act
                var result = queue.Summarize();

                // Assert
                Assert.That(result.ToArray(), Is.EquivalentTo(new[]
                {
                    new FinalizedChange(ChangeAction.Delete, "a")
                }));
            }

            [Test]
            public void Delete_a_file_then_update_it()
            {
                // Arrange
                var changes = new[]
                {
                    new Change(ChangeAction.Delete, "a"),
                    new Change(ChangeAction.Update, "a")
                };
                var queue = new TestableChangeQueue(changes);

                // Act
                var result = queue.Summarize();

                // Assert
                Assert.That(result.ToArray(), Is.EquivalentTo(new[]
                {
                    new FinalizedChange(ChangeAction.Update, "a")
                }));
            }

            [Test]
            public void Deleting_a_folder_after_a_file_inside_it_changes_should_only_consider_the_deletion()
            {
                // Arrange
                var changes = new[]
                {
                    new Change(ChangeAction.Update, "models\\product.json"),
                    new Change(ChangeAction.Delete, "models")
                };
                var client = new TestableChangeQueue(changes);

                // Act
                var result = client.Summarize();

                // Assert
                Assert.That(result.ToArray(), Is.EquivalentTo(new[]
                {
                    new FinalizedChange(ChangeAction.Delete, "models")
                }));
            }

            [Test]
            public void Updating_a_file_inside_a_folder_that_was_deleted_should_send_both()
            {
                // Arrange
                var changes = new[]
                {
                    new Change(ChangeAction.Delete, "models"),
                    new Change(ChangeAction.Update, "models\\product.json")
                };
                var client = new TestableChangeQueue(changes);

                // Act
                var result = client.Summarize();

                // Assert
                Assert.That(result.ToArray(), Is.EquivalentTo(new[]
                {
                    new FinalizedChange(ChangeAction.Delete, "models"),
                    new FinalizedChange(ChangeAction.Update, "models\\product.json")
                }));
            }
        }
    }

    public class TestableChangeQueue : ChangeQueue
    {
        public TestableChangeQueue(IEnumerable<Change> changes) : base(null)
        {
            foreach (var change in changes)
                Enqueue(change);
        }

        protected override FinalizedChange FinalizeUpdate(string fullPath)
        {
            return FinalizedChange.ForUpdate(fullPath, "");
        }

        protected override FinalizedChange FinalizeDeletion(string fullPath)
        {
            return FinalizedChange.ForDeletion(fullPath);
        }
    }
}
