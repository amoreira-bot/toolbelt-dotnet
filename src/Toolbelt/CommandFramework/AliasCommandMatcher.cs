using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Vtex.Toolbelt.CommandFramework
{
    public class AliasCommandMatcher : ICommandMatcher
    {
        private readonly IEnumerable<Type> _candidateTypes;

        public AliasCommandMatcher(IEnumerable<Type> candidateTypes)
        {
            _candidateTypes = candidateTypes;
        }

        public bool TryGetMatchedType(string[] args, out Type commandType, out int usedArgCount)
        {
            usedArgCount = 1;
            if (!args.Any())
            {
                commandType = null;
                return false;
            }

            var alias = args[0].ToLower();
            commandType = _candidateTypes.FirstOrDefault(type => GetAlias(type) == alias);
            return commandType != null;
        }

        private static string GetAlias(Type type)
        {
            var helpAttribute = type.GetCustomAttribute<CommandHelpAttribute>(true);
            return helpAttribute == null || helpAttribute.Alias == null ? null : helpAttribute.Alias.ToLower();
        }
    }
}