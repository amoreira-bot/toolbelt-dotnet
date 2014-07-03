using System;
using System.IO;
using System.Text;
using IniParser.Exceptions;
using IniParser.Model;
using Vtex.Toolbelt.Core;

namespace Vtex.Toolbelt.Cli
{
    public class ConfigurationReader
    {
        private readonly string filePath;

        public ConfigurationReader(string filePath)
        {
            this.filePath = filePath;
        }

        public Configuration ReadConfiguration()
        {
            var configuration = new Configuration();

            var iniData = this.ReadIniData();
            if (iniData == null)
                return configuration;

            LoadFileSystemSection(iniData, configuration);
            LoadGallerySection(iniData, configuration);

            return configuration;
        }

        private static void LoadFileSystemSection(IniData iniData, Configuration configuration)
        {
            var section = iniData.Sections["fs"];
            if (section == null)
                return;

            int delay;
            if (int.TryParse(section["delay"], out delay))
                configuration.FileSystemDelay = delay;
        }

        private static void LoadGallerySection(IniData iniData, Configuration configuration)
        {
            var section = iniData.Sections["gallery"];
            if (section == null)
                return;

            var endpoint = section["endpoint"];
            if (!string.IsNullOrWhiteSpace(endpoint))
                configuration.GalleryEndpoint = endpoint;
        }

        private IniData ReadIniData()
        {
            var parser = new IniParser.FileIniDataParser();
            try
            {
                return parser.ReadFile(this.filePath, Encoding.UTF8);
            }
            catch (ParsingException exception)
            {
                var ioException = exception.InnerException as IOException;
                if (ioException != null)
                    return null;

                throw new InvalidOperationException("Error reading configuration file. " + exception.Message);
            }
        }
    }
}
