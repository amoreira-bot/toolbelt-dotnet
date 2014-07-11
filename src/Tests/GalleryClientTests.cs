using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Vtex.Toolbelt.Core;

namespace Vtex.Toolbelt.Tests
{
    [TestFixture]
    public class GalleryClientTests
    {
        class SummarizeChanges
        {
            [Test]
            public void Update_one_file()
            {
                // Arrange
                var client = new TestableGalleryClient("");
                var changes = new[]
                {
                    new Change(ChangeAction.Update, "a")
                };

                // Act
                var result = client.PubliclySummarizeChanges(changes);

                // Assert
                Assert.That(result, Is.EquivalentTo(changes));
            }

            [Test]
            public void Update_same_file_twice()
            {
                // Arrange
                var client = new TestableGalleryClient("");
                var changes = new[]
                {
                    new Change(ChangeAction.Update, "a"),
                    new Change(ChangeAction.Update, "a")
                };

                // Act
                var result = client.PubliclySummarizeChanges(changes);

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
                var client = new TestableGalleryClient("");
                var changes = new[]
                {
                    new Change(ChangeAction.Delete, "a")
                };

                // Act
                var result = client.PubliclySummarizeChanges(changes);

                // Assert
                Assert.That(result, Is.EquivalentTo(changes));
            }

            [Test]
            public void Delete_same_file_twice()
            {
                // Arrange
                var client = new TestableGalleryClient("");
                var changes = new[]
                {
                    new Change(ChangeAction.Delete, "a"),
                    new Change(ChangeAction.Delete, "a")
                };

                // Act
                var result = client.PubliclySummarizeChanges(changes);

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
                var client = new TestableGalleryClient("");
                var changes = new[]
                {
                    new Change(ChangeAction.Update, "a"),
                    new Change(ChangeAction.Delete, "a")
                };

                // Act
                var result = client.PubliclySummarizeChanges(changes);

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
                var client = new TestableGalleryClient("");
                var changes = new[]
                {
                    new Change(ChangeAction.Delete, "a"),
                    new Change(ChangeAction.Update, "a")
                };

                // Act
                var result = client.PubliclySummarizeChanges(changes);

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
                var client = new TestableGalleryClient("");
                var changes = new[]
                {
                    new Change(ChangeAction.Update, "models\\product.json"),
                    new Change(ChangeAction.Delete, "models")
                };

                // Act
                var result = client.PubliclySummarizeChanges(changes);

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
                var client = new TestableGalleryClient("");
                var changes = new[]
                {
                    new Change(ChangeAction.Delete, "models"),
                    new Change(ChangeAction.Update, "models\\product.json")
                };

                // Act
                var result = client.PubliclySummarizeChanges(changes);

                // Assert
                Assert.That(result.ToArray(), Is.EquivalentTo(new[]
                {
                    new Change(ChangeAction.Delete, "models"),
                    new Change(ChangeAction.Update, "models\\product.json")
                }));
            }
        }
    }

    public class TestableGalleryClient : GalleryClient
    {
        public TestableGalleryClient(string rootPath)
            : base(null, null, rootPath, null, "http://something.com/")
        {
        }

        public IEnumerable<Change> PubliclySummarizeChanges(IEnumerable<Change> changes)
        {
            return this.SummarizeChanges(changes);
        }

        protected override string NormalizePath(string path)
        {
            return path;
        }
    }
}
