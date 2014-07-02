using CommandLine;

namespace Vtex.Toolbelt.Cli
{
    public class SyncVerbObtions
    {
        [Option('a', "account", Required = true, HelpText = "Account name")]
        public string Account { get; set; }

        [Option('w', "workspace", Required = true, HelpText = "Workspace name")]
        public string Workspace { get; set; }
    }
}