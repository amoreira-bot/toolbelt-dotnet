using System;

namespace Vtex.Toolbelt.CommandFramework
{
    public class NamedOption : Option
    {
        public string Name { get; private set; }

        public char Shorthand { get; private set; }

        public NamedOption(string name, char shorthand, string description, bool required, Action<string> apply)
            :base(description, required, apply)
        {
            Name = name;
            Shorthand = shorthand;
        }
    }
}