using System;
using System.Collections.Generic;
using System.Linq;

namespace Vtex.Toolbelt.CommandFramework
{
    public class CompositeCommandMatcher : ICommandMatcher
    {
        private readonly ICommandMatcher _firstMatcher;
        private readonly ICommandMatcher _secondMatcher;

        public IEnumerable<Type> CommandTypes
        {
            get { return _firstMatcher.CommandTypes.Union(_secondMatcher.CommandTypes).Distinct(); }
        }

        public CompositeCommandMatcher(ICommandMatcher firstMatcher, ICommandMatcher secondMatcher)
        {
            _firstMatcher = firstMatcher;
            _secondMatcher = secondMatcher;
        }

        public bool TryGetMatchedType(string[] args, out Type commandType, out int usedArgCount)
        {
            return _firstMatcher.TryGetMatchedType(args, out commandType, out usedArgCount)
                   || _secondMatcher.TryGetMatchedType(args, out commandType, out usedArgCount);
        }
    }
}