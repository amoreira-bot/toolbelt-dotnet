using System;

namespace Vtex.Toolbelt.CommandFramework
{
    public class AliasAttribute : Attribute
    {
        public string Alias { get; private set; }

        public AliasAttribute(string @alias)
        {
            Alias = alias;
        }
    }
}