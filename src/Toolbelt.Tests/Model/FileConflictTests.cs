using NUnit.Framework;
using Vtex.Toolbelt.Model;

namespace Vtex.Toolbelt.Tests.Model
{
    [TestFixture]
    class FileConflictTests
    {
        class HumanizeBytes
        {
            [Test]
            public void Returns_dash_when_null()
            {
                // Act
                var humanized = FileConflict.HumanizeBytes(null);

                // Assert
                Assert.That(humanized, Is.EqualTo("-"));
            }

            [TestCase(0, "0 B")]
            [TestCase(45, "45 B")]
            [TestCase(1023, "1023 B")]
            public void Returns_humanized_bytes(int size, string expected)
            {
                // Act
                var humanized = FileConflict.HumanizeBytes(size);

                // Assert
                Assert.That(humanized, Is.EqualTo(expected));
            }

            [TestCase(1024, "1 KB")]
            [TestCase(1536, "1.5 KB")]
            [TestCase(1820, "1.8 KB")]
            [TestCase(1048515, "1023.9 KB")]
            public void Returns_humanized_kilobytes(int size, string expected)
            {
                // Act
                var humanized = FileConflict.HumanizeBytes(size);

                // Assert
                Assert.That(humanized, Is.EqualTo(expected));
            }

            [TestCase(1048576, "1 MB")]
            [TestCase(1572864, "1.5 MB")]
            [TestCase(497659202, "474.6 MB")]
            [TestCase(1073741824, "1024 MB")]
            public void Returns_humanized_megabytes(int size, string expected)
            {
                // Act
                var humanized = FileConflict.HumanizeBytes(size);

                // Assert
                Assert.That(humanized, Is.EqualTo(expected));
            }
        }
    }
}
