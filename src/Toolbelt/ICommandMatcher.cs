using System;

namespace Vtex.Toolbelt
{
    public interface ICommandMatcher
    {
        bool TryGetMatchedType(string[] args, out Type commandType, out int usedArgCount);
    }
}