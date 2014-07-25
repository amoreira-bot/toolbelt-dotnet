using System;
using System.Net.Http;

namespace Vtex.Toolbelt.Services
{
    public class ApiException : ApplicationException
    {
        public ApiException(string message, HttpResponseMessage response) : base(message)
        {
        }
    }
}
