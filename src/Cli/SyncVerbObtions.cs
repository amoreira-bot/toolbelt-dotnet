using CommandLine;
using CommandLine.Text;

namespace Vtex.Toolbelt.Cli
{
    public class SyncVerbObtions
    {
        [Option('a', "account", Required = true, HelpText = "Account name")]
        public string Account { get; set; }

        [Option('s', "session", Required = true, HelpText = "Session name")]
        public string Session { get; set; }
    }
}