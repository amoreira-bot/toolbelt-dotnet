using System;

namespace Vtex.Toolbelt.CommandFramework
{
    public class ParameterOption : Option
    {
        public int Position { get; private set; }

        public string Name { get; private set; }

        public ParameterOption(int position, string name, string description, bool required, Action<string> apply)
            : base(description, required, apply)
        {
            Position = position;
            Name = name;
        }
    }
}