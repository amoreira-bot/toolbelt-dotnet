using System;

namespace Vtex.Toolbelt.CommandFramework
{
    public interface ICommandMatcher
    {
        bool TryGetMatchedType(string[] args, out Type commandType, out int usedArgCount);
    }
}