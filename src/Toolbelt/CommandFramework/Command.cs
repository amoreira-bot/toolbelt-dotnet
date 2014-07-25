using System.Linq;

namespace Vtex.Toolbelt.CommandFramework
{
    public abstract class Command : ICommand
    {
        protected readonly IConsole Console;
        protected readonly OptionSet OptionSet = new OptionSet();
        private bool _helpCommand;

        protected Command(IConsole console)
        {
            Console = console;
            OptionSet.Add("help", 'h', "display this help screen", () => _helpCommand = true);
        }

        public void Execute(string invokedWith, string[] args)
        {
            OptionSet.Parse(args);
            if (_helpCommand)
            {
                WriteUsage(invokedWith);
                return;
            }
            InnerExecute();
        }

        protected abstract void InnerExecute();

        private void WriteUsage(string invokedWith)
        {
            var commandHelp = this.GetType().GetCustomAttributes(true).OfType<CommandHelpAttribute>().Single();
            var writer = Console.Out;

            writer.WriteLine("usage: vtex {0} {1} [options]", invokedWith, OptionSet.GetParametersUsage());
            writer.WriteLine(commandHelp.Description);

            writer.WriteLine();
            OptionSet.WriteNamedOptionsUsage(writer);

            writer.WriteLine();
            writer.WriteLine();
        }
    }
}