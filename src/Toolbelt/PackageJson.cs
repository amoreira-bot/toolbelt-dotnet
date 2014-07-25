using System;

namespace Vtex.Toolbelt
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
    }
}