using NUnit.Framework;
using Vtex.Toolbelt.Model;
using Vtex.Toolbelt.Services;

namespace Vtex.Toolbelt.Tests.Services
{
    [TestFixture]
    public class WatcherTests
    {
        class OnCreated
        {
            [Test]
            public void Creating_a_file_should_update_it()
            {
                // Arrange
                var watcher = new TestableWatcher();

                // Act
                watcher.PubliclyOnCreated("foo");

                // Assert
                Assert.That(watcher.UpdatedPaths, Is.EquivalentTo(new[] {"foo"}));
            }

            [Test]
            public void Creating_a_folder_should_update_all_files_inside_it()
            {
                // Arrange
                var watcher = new TestableWatcher();
                watcher.AddFolder("foofolder", new[] {"um", "dois", "três"});

                // Act
                watcher.PubliclyOnCreated("foofolder");

                // Assert
                Assert.That(watcher.UpdatedPaths, Is.EquivalentTo(new[] {"um", "dois", "três"}));
            }
        }

        class OnChanged
        {
            [Test]
            public void Updating_a_file()
            {
                // Arrange
                var watcher = new TestableWatcher();

                // Act
                watcher.PubliclyOnChanged("blabla");

                // Assert
                Assert.That(watcher.UpdatedPaths, Is.EquivalentTo(new[] {"blabla"}));
            }

            [Test]
            public void Updating_a_folder_should_do_nothing()
            {
                // Arrange
                var watcher = new TestableWatcher();
                watcher.AddFolder("somefolder", new[] {"um", "dois", "três"});

                // Act
                watcher.PubliclyOnChanged("somefolder");

                // Assert
                Assert.That(watcher.UpdatedPaths, Is.Empty);
            }
        }

        class OnDeleted
        {
            [Test]
            public void Deleting_a_file()
            {
                // Arrange
                var watcher = new TestableWatcher();

                // Act
                watcher.PubliclyOnDeleted("foo");

                // Assert
                Assert.That(watcher.DeletedPaths, Is.EquivalentTo(new[] {"foo"}));
            }

            [Test]
            public void Deleting_a_folder_should_just_delete_it()
            {
                // Arrange
                var watcher = new TestableWatcher();

                // Act
                watcher.PubliclyOnDeleted("foofolder");

                // Assert
                Assert.That(watcher.DeletedPaths, Is.EquivalentTo(new[] { "foofolder" }));
            }
        }

        class UpdatePath
        {
            [Test]
            public void Add_a_file_to_be_updated()
            {
                // Arrange
                var watcher = new TestableWatcher();

                // Act
                watcher.PubliclyUpdatePath("foo");

                // Assert
                Assert.That(watcher.Changes, Has.Length.EqualTo(1));
                Assert.That(watcher.Changes[0].Action, Is.EqualTo(ChangeAction.Update));
                Assert.That(watcher.Changes[0].Path, Is.EqualTo("foo"));
            }

            [Test]
            public void Trigger_a_submission_request()
            {
                // Arrange
                var watcher = new TestableWatcher();

                // Act
                watcher.PubliclyUpdatePath("foo");

                // Assert
                Assert.That(watcher.DebounceCount, Is.EqualTo(1));
            }
        }

        class DeletePath
        {
            [Test]
            public void Add_a_path_to_be_deleted()
            {
                // Arrange
                var watcher = new TestableWatcher();

                // Act
                watcher.PubliclyDeletePath("foo");

                // Assert
                Assert.That(watcher.Changes, Has.Length.EqualTo(1));
                Assert.That(watcher.Changes[0].Action, Is.EqualTo(ChangeAction.Delete));
                Assert.That(watcher.Changes[0].Path, Is.EqualTo("foo"));
            }

            [Test]
            public void Trigger_a_submission_request()
            {
                // Arrange
                var watcher = new TestableWatcher();

                // Act
                watcher.PubliclyDeletePath("foo");

                // Assert
                Assert.That(watcher.DebounceCount, Is.EqualTo(1));
            }
        }
    }
}
