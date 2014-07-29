using System;

namespace Vtex.Toolbelt.CommandFramework
{
    public class OptionsValidationException : Exception
    {
        public OptionsValidationException(string message) : base(message)
        {
        }
    }
}