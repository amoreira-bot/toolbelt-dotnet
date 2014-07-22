using System;

namespace Vtex.Toolbelt
{
    public class CommandHelpAttribute : Attribute
    {
        public string Alias { get; set; }
    }
}