using System;

namespace Vtex.Toolbelt.CommandFramework
{
    public class DispatchException : Exception
    {
        public DispatchException(string message)
            : base(message)
        {
        }
    }
}