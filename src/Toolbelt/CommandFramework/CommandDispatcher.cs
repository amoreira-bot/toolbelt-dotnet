using System;
using System.Linq;

namespace Vtex.Toolbelt.CommandFramework
{
    public class CommandDispatcher
    {
        private readonly ICommandMatcher _commandMatcher;
        private readonly IServiceProvider _services;

        public CommandDispatcher(ICommandMatcher commandMatcher, IServiceProvider services)
        {
            _commandMatcher = commandMatcher;
            _services = services;
        }

        public void Dispatch(string[] args)
        {
            Type commandType;
            int usedArgCount;
            if (!_commandMatcher.TryGetMatchedType(args, out commandType, out usedArgCount))
            {
                throw new DispatchException(string.Format("The command \"{0}\" doesn't match a command or alias",
                    string.Join(" ", args)));
            }

            var command = this.CreateCommand(commandType);
            var commandArgs = args.Skip(usedArgCount).ToArray();
            try
            {
                command.Execute(string.Join(" ", args.Take(usedArgCount)), commandArgs);
            }
            catch (Exception exception)
            {
                throw new DispatchException(exception.Message);
            }
        }

        protected virtual ICommand CreateCommand(Type commandType)
        {
            return (ICommand) _services.GetService(commandType);
        }
    }
}