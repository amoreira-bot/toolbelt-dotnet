namespace Vtex.Toolbelt.Core
{
    using System;

    public class PackageJson
    {
        public string Name { get; set; }

        public void Validate()
        {
            if (String.IsNullOrEmpty(this.Name))
            {
                throw new ApplicationException("Account name is required. Check the package.json file.");
            }
        }
    }
}
