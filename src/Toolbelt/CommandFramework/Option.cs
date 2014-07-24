using System;

namespace Vtex.Toolbelt.CommandFramework
{
    public class Option
    {
        public string Description { get; private set; }

        public bool Required { get; private set; }

        public Action<string> Apply { get; private set; }

        public Option(string description, bool required, Action<string> apply)
        {
            Description = description;
            Required = required;
            Apply = apply;
        }
    }
}