using System;

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
            Console.WriteLine(Environment.CurrentDirectory);

            Console.WriteLine("Press \'q\' to quit.");
            while (Console.Read() != 'q') ;
        }
    }
}