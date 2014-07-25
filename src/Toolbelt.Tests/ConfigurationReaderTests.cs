using System;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace Vtex.Toolbelt.Tests
{
    public class ConfigurationReaderTests
    {
        [Test]
        public void Keeps_all_default_values_when_file_is_empty()
        {
            // Arrange
            var reader = new TestableConfigurationReader(string.Empty);

            // Act
            var configuration = reader.ReadConfiguration();

            // Assert
            Assert.That(configuration.FileSystemDelay, Is.EqualTo(Configuration.DefaultFileSystemDelay));
            Assert.That(configuration.GalleryEndpoint, Is.EqualTo(Configuration.DefaultGalleryEndpoint));
        }

        [Test]
        public void Keeps_all_default_values_when_file_does_not_exist()
        {
            // Arrange
            var reader = new TestableConfigurationReader(() => { throw new FileNotFoundException(); });

            // Act
            var configuration = reader.ReadConfiguration();

            // Assert
            Assert.That(configuration.FileSystemDelay, Is.EqualTo(Configuration.DefaultFileSystemDelay));
            Assert.That(configuration.GalleryEndpoint, Is.EqualTo(Configuration.DefaultGalleryEndpoint));
        }

        [Test]
        public void Sets_gallery_endpoint_value()
        {
            // Arrange
            var file = @"
[gallery]
endpoint = some other url
";
            var reader = new TestableConfigurationReader(file);

            // Act
            var configuration = reader.ReadConfiguration();

            // Assert
            Assert.That(configuration.GalleryEndpoint, Is.EqualTo("some other url"));
        }

        [Test]
        public void Sets_file_system_delay_value()
        {
            var file = @"
[fs]
delay = 999
";
            var reader = new TestableConfigurationReader(file);

            // Act
            var configuration = reader.ReadConfiguration();

            // Assert
            Assert.That(configuration.FileSystemDelay, Is.EqualTo(999));
        }

        [Test]
        public void Uses_default_value_when_delay_configuration_is_invalid()
        {
            var file = @"
[fs]
delay = this is not a number
";
            var reader = new TestableConfigurationReader(file);

            // Act
            var configuration = reader.ReadConfiguration();

            // Assert
            Assert.That(configuration.FileSystemDelay, Is.EqualTo(Configuration.DefaultFileSystemDelay));
        }

        [Test]
        public void Throws_exception_when_file_is_in_an_incorrect_format()
        {
            var file = @"
[fs
invalid file format
";
            var reader = new TestableConfigurationReader(file);

            // Act & Assert
            Assert.That(() => reader.ReadConfiguration(), Throws.TypeOf<ConfigurationException>());
        }

        [Test]
        public void Throws_configuration_exception_when_some_other_exception_occurs()
        {
            var reader = new TestableConfigurationReader(() => { throw new Exception("Unexpected error"); });

            // Act & Assert
            Assert.That(() => reader.ReadConfiguration(), Throws.TypeOf<ConfigurationException>());
        }

        class TestableConfigurationReader : ConfigurationReader
        {
            private readonly Func<StreamReader> _getFileReader;

            public TestableConfigurationReader(string rawConfigurationFile) : base(string.Empty)
            {
                _getFileReader = () => new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(rawConfigurationFile)));
            }

            public TestableConfigurationReader(Func<StreamReader> getFileReader)
                : base(string.Empty)
            {
                _getFileReader = getFileReader;
            }

            protected override StreamReader GetFileReader()
            {
                return _getFileReader();
            }
        }
    }
}
