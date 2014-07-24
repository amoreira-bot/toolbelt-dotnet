using System;

namespace Vtex.Toolbelt.CommandFramework
{
    public class OptionSyntaxException : Exception
    {
        public OptionSyntaxException(string message) : base(message)
        {
        }
    }
}