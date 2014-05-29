using System;
using System.IO;
using Vtex.Toolbelt.Core;

namespace Vtex.Toolbelt.Cli.Commands
{
    public class SyncCommand : Command
    {
        private readonly string accountName;
        private readonly string sessionName;

        public SyncCommand(SyncVerbObtions options)
        {
            this.accountName = options.Account;
            this.sessionName = options.Session;
        }

        public override void Run()
        {
            Console.WriteLine("Run for account '{0}' and session '{1}'", accountName, sessionName);

            var rootPath = Path.Combine(Environment.CurrentDirectory, this.accountName);
            Console.WriteLine("Watching '{0}' for changes", rootPath);

            var watcher = new Watcher(this.accountName, this.sessionName, rootPath);
            watcher.Start();

            Console.WriteLine("Press \'q\' to quit.");
            while (Console.Read() != 'q') ;
        }
    }
}