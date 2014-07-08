using CommandLine;

namespace Vtex.Toolbelt.Cli
{
    public class SyncVerbOptions
    {
        [Option('w', "workspace", Required = true, HelpText = "Workspace name")]
        public string Workspace { get; set; }
    }
}