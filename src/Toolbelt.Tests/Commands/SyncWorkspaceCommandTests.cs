using NUnit.Framework;
using Vtex.Toolbelt.Commands;

namespace Vtex.Toolbelt.Tests.Commands
{
    class SyncWorkspaceCommandTests
    {
        [Test]
        public void Should_have_valid_options()
        {
            // Arrange
            var command = new SyncWorkspaceCommand(null, null, null, null, null);

            // Act & Assert
            Assert.That(() => command.OptionSet.Validate(), Throws.Nothing);
        }
    }
}
