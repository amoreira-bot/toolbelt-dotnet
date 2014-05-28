using System;
using CommandLine;
using Vtex.Toolbelt.Cli.Commands;

namespace Vtex.Toolbelt.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            var parser = new Parser(config =>
            {
                config.CaseSensitive = true;
                config.IgnoreUnknownArguments = false;
                config.CaseSensitive = true;
                config.HelpWriter = Console.Out;
            });

            string verb = "";
            object subOptions = null;
            var options = new Options();
            if (!parser.ParseArgumentsStrict(args, options, (cmd, opts) =>
            {
                verb = cmd;
                subOptions = opts;
            }))
            {
                Environment.Exit(Parser.DefaultExitCodeFail);
            }

            Command command;
            switch (verb)
            {
                case "sync":
                    command = new SyncCommand((SyncVerbObtions) subOptions);
                    break;

                default:
                    throw new Exception("Invalid command " + verb);
            }

            command.Run();
        }
    }
}
