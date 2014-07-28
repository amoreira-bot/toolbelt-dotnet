using System;
using System.Collections.Generic;
using System.Linq;

namespace Vtex.Toolbelt.CommandFramework
{
    public class TypeNameCommandMatcher : ICommandMatcher
    {
        private readonly IEnumerable<Type> _candidateTypes;

        public IEnumerable<Type> CommandTypes
        {
            get { return _candidateTypes; }
        }

        public TypeNameCommandMatcher(IEnumerable<Type> candidateTypes)
        {
            _candidateTypes = candidateTypes;
        }

        public bool TryGetMatchedType(string[] args, out Type commandType, out int usedArgCount)
        {
            var commandArguments = args.TakeWhile(arg => !arg.StartsWith("-")).Take(2).ToArray();
            var commandName = commandArguments.Any()
                ? string.Concat(commandArguments.Skip(1).FirstOrDefault(), commandArguments[0])
                : "help";
            var typeName = string.Concat(commandName, "command").ToLower();

            commandType = _candidateTypes.FirstOrDefault(type => type.Name.ToLower() == typeName);
            usedArgCount = commandArguments.Length;
            return commandType != null;
        }
    }
}