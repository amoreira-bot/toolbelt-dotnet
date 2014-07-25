using System;

namespace Vtex.Toolbelt.CommandFramework
{
    public class CommandHelpAttribute : Attribute
    {
        public string Description { get; private set; }

        public string Alias { get; private set; }

        public CommandHelpAttribute(string description, string @alias = null)
        {
            Description = description;
            Alias = alias;
        }
    }
}