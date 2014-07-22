using System;

namespace Vtex.Toolbelt
{
    public class DispatchException : Exception
    {
        public DispatchException(string message)
            : base(message)
        {
        }
    }
}