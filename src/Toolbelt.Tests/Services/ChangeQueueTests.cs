﻿using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
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
                var result = queue.Summarize("");

                // Assert
                Assert.That(result, Is.EquivalentTo(changes));
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
                var result = queue.Summarize("");

                // Assert
                Assert.That(result.ToArray(), Is.EquivalentTo(new[]
                {
                    new Change(ChangeAction.Update, "a")
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
                var result = queue.Summarize("");

                // Assert
                Assert.That(result, Is.EquivalentTo(changes));
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
                var result = queue.Summarize("");

                // Assert
                Assert.That(result.ToArray(), Is.EquivalentTo(new[]
                {
                    new Change(ChangeAction.Delete, "a")
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
                var result = queue.Summarize("");

                // Assert
                Assert.That(result.ToArray(), Is.EquivalentTo(new[]
                {
                    new Change(ChangeAction.Delete, "a")
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
                var result = queue.Summarize("");

                // Assert
                Assert.That(result.ToArray(), Is.EquivalentTo(new[]
                {
                    new Change(ChangeAction.Update, "a")
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
                var result = client.Summarize("");

                // Assert
                Assert.That(result.ToArray(), Is.EquivalentTo(new[]
                {
                    new Change(ChangeAction.Delete, "models")
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
                var result = client.Summarize("");

                // Assert
                Assert.That(result.ToArray(), Is.EquivalentTo(new[]
                {
                    new Change(ChangeAction.Delete, "models"),
                    new Change(ChangeAction.Update, "models\\product.json")
                }));
            }

            [Test]
            public void Should_filter_ignored_files()
            {
                // Arrange
                var changes = new[]
                {
                    new Change(ChangeAction.Update, "should-not-be-ignored.txt"),
                    new Change(ChangeAction.Delete, "4790"),
                    new Change(ChangeAction.Update, ".gitignore"),
                    new Change(ChangeAction.Update, "inner\\.gitignore"),
                    new Change(ChangeAction.Update, "inner\\.git\\config"),
                    new Change(ChangeAction.Delete, "some-file.txt.tmp"),
                    new Change(ChangeAction.Delete, "some-file.txt.1234.TMP"),
                    new Change(ChangeAction.Delete, "some-file.txt~"),
                    new Change(ChangeAction.Delete, "4913"),
                    new Change(ChangeAction.Delete, "5036"),
                    new Change(ChangeAction.Delete, "inner\\4913"),
                };
                var queue = new TestableChangeQueue(changes);

                // Act
                var result = queue.Summarize("");

                // Assert
                Assert.That(result, Is.EquivalentTo(new[]
                {
                    new Change(ChangeAction.Update, "should-not-be-ignored.txt"),
                    new Change(ChangeAction.Delete, "4790"),
                }));
            }
        }
    }

    public class TestableChangeQueue : ChangeQueue
    {
        public TestableChangeQueue(IEnumerable<Change> changes)
        {
            foreach (var change in changes)
                Enqueue(change);
        }

        protected override string NormalizePath(string path, string rootPath)
        {
            return path;
        }
    }
}
