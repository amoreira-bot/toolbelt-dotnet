using System;
using System.IO;
using System.Text;
using IniParser.Exceptions;
using IniParser.Model;

namespace Vtex.Toolbelt
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
            var parser = new IniParser.StreamIniDataParser();
            try
            {
                using (var reader = GetFileReader())
                    return parser.ReadData(reader);
            }
            catch (FileNotFoundException)
            {
                return null;
            }
            catch (ParsingException exception)
            {
                throw new ConfigurationException(exception.Message);
            }
            catch (Exception exception)
            {
                throw new ConfigurationException("Unexpected error when reading configuration: " + exception.Message);
            }
        }

        protected virtual StreamReader GetFileReader()
        {
            return new StreamReader(File.OpenRead(this.filePath), Encoding.UTF8);
        }
    }
}
