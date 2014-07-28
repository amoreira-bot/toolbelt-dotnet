using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Vtex.Toolbelt.CommandFramework;

namespace Vtex.Toolbelt.Commands
{
    [CommandHelp("display this help screen")]
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
            var commands = _commandMatcher.CommandTypes.ToDictionary(GetCommandName,
                t => t.GetCustomAttribute<CommandHelpAttribute>());

            var columnWidth = commands.Keys.Select(k => k.Length).Max() + 3;
            foreach (var command in commands.OrderBy(c => c.Key))
            {
                Console.WriteLine("    [#white {0}]{1}",
                    command.Key.PadRight(columnWidth),
                    (string.IsNullOrWhiteSpace(command.Value.Alias)
                        ? command.Value.Description
                        : string.Format("([#white {0}]) {1}", command.Value.Alias, command.Value.Description)));
            }

            Console.WriteLine();
            Console.WriteLine("For additional help on a command, call it with the --help flag.");
            Console.WriteLine();
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
