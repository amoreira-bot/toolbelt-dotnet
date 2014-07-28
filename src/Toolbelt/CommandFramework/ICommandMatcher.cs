using System;
using System.Collections.Generic;

namespace Vtex.Toolbelt.CommandFramework
{
    public interface ICommandMatcher
    {
        IEnumerable<Type> CommandTypes { get; }

        bool TryGetMatchedType(string[] args, out Type commandType, out int usedArgCount);
    }
}