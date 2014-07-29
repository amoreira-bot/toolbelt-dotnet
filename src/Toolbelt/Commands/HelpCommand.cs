using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Vtex.Toolbelt.CommandFramework;

namespace Vtex.Toolbelt.Commands
{
    [Description("display this help screen")]
    public class HelpCommand : Command
    {
        private readonly ICommandMatcher _commandMatcher;

        public HelpCommand(IConsole console, ICommandMatcher commandMatcher)
            : base(console)
        {
            _commandMatcher = commandMatcher;
        }

        protected override void InnerExecute()
        {
            var version = Assembly.GetExecutingAssembly()
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                .InformationalVersion;

            Console.WriteLine("VTEX Toolbelt v" + version);
            Console.WriteLine();

            System.Console.WriteLine("Usage: vtex <command> [options]");
            Console.WriteLine();

            Console.WriteLine("These are the available commands:");
            var commands = _commandMatcher.CommandTypes.Select(CommandHelp.FromType).ToArray();

            var columnWidth = commands.Select(c => c.Name.Length).Max() + 3;
            foreach (var command in commands.OrderBy(c => c.Name))
            {
                var description = command.Alias == null
                    ? command.Description
                    : string.Format("([#white {0}]) {1}", command.Alias, command.Description);
                Console.WriteLine("    [#white {0}]{1}", command.Name.PadRight(columnWidth), description);
            }

            Console.WriteLine();
            Console.WriteLine("For additional help on a command, call it with the --help flag.");
            Console.WriteLine();
        }

        private class CommandHelp
        {
            public string Name { get; private set; }
            public string Description { get; private set; }
            public string Alias { get; private set; }

            public static CommandHelp FromType(Type commandType)
            {
                var descriptionAttribute = commandType.GetCustomAttribute<DescriptionAttribute>();
                var aliasAttribute = commandType.GetCustomAttribute<AliasAttribute>();

                return new CommandHelp
                {
                    Name = GetCommandName(commandType),
                    Description = descriptionAttribute != null ? descriptionAttribute.Description : string.Empty,
                    Alias = aliasAttribute != null ? aliasAttribute.Alias : null
                };
            }

            private static string GetCommandName(Type type)
            {
                var parts = Regex.Replace(type.Name, "([A-Z])", "_$1").ToLower()
                    .Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries)
                    .Reverse().Skip(1);
                return string.Join(" ", parts);
            }
        }
    }
}
