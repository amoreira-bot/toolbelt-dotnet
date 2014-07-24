using System;

namespace Vtex.Toolbelt.CommandFramework
{
    public class FlagOption : NamedOption
    {
        public FlagOption(string name, char shorthand, string description, Action apply)
            :base(name, shorthand, description, false, value => apply())
        {
        }
    }
}