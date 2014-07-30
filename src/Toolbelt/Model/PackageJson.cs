using Newtonsoft.Json;
using System;
using System.IO;
using Vtex.Toolbelt.Services;

namespace Vtex.Toolbelt.Model
{
    public class PackageJson
    {
        public string Name { get; set; }

        public void Validate()
        {
            if (String.IsNullOrEmpty(this.Name))
            {
                throw new ApplicationException("Field \"name\" should be set in package.json file");
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