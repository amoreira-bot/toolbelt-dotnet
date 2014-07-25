using System;

namespace Vtex.Toolbelt
{
    public class ConfigurationException : ApplicationException
    {
        public ConfigurationException(string message) : base(message)
        {
        }
    }
}