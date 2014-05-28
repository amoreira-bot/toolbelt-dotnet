using CommandLine;
using CommandLine.Text;

namespace Vtex.Toolbelt.Cli
{
    public class Options
    {
        [VerbOption("sync", HelpText = "Start synchronization job with a session")]
        public SyncVerbObtions SyncVerb { get; set; }

        [HelpVerbOption("help")]
        public string Help(string verb)
        {
            return HelpText.AutoBuild(this, verb);
        }
    }
}