using System;
using System.IO;
using CommandLine;
using Vtex.Toolbelt.Cli.Commands;
using Vtex.Toolbelt.Core;

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

            var configuration = ReadConfiguration();

            Command command;
            switch (verb)
            {
                case "sync":
                    command = new SyncCommand((SyncVerbOptions) subOptions, configuration);
                    break;

                default:
                    throw new Exception("Invalid command " + verb);
            }

            command.Run();
        }

        private static Configuration ReadConfiguration()
        {
            var home = Environment.OSVersion.Platform == PlatformID.MacOSX ||
                       Environment.OSVersion.Platform == PlatformID.Unix
                ? Environment.GetEnvironmentVariable("HOME")
                : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");

            var configFilePath = Path.Combine(home, ".vtexrc");
            var configurationReader = new ConfigurationReader(configFilePath);
            return configurationReader.ReadConfiguration();
        }
    }
}
