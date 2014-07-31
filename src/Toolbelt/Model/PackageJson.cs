using Newtonsoft.Json;
using System;
using System.IO;
using Vtex.Toolbelt.Services;
using System.Text.RegularExpressions;

namespace Vtex.Toolbelt.Model
{
    public class PackageJson
    {
        public string Name { get; set; }
        public string Version { get; set; }

        public void Validate()
        {
            if (String.IsNullOrEmpty(this.Name))
            {
                throw new ApplicationException("Field \"name\" should be set in package.json file");
            }

            if (String.IsNullOrEmpty(this.Version))
            {
                throw new ApplicationException("Field \"version\" should be set in package.json file");
            }

            var match = Regex.Match(this.Version, @"(\d+)\.(\d+)\.(\d+)");
            if (!match.Success)
            {
                throw new ApplicationException("The version format is invalid");
            }
        }

        public static PackageJson In(IFileSystem fileSystem)
        {
            try
            {
                var json = fileSystem.ReadTextFile("package.json");
                return JsonConvert.DeserializeObject<PackageJson>(json);
            }
            catch (FileNotFoundException)
            {
                throw new ApplicationException("Couldn't find package.json file.");
            }
        }
    }
}