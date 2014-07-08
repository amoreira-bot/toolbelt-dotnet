using System;
using System.Collections.Generic;
using System.IO;
using Vtex.Toolbelt.Core;

namespace Vtex.Toolbelt.Cli.Commands
{
    public class SyncCommand : Command
    {
        private readonly Configuration configuration;
        private readonly string accountName;
        private readonly string workspaceName;
        private readonly LoginCommand loginCommand;

        public SyncCommand(SyncVerbObtions options, Configuration configuration)
        {
            this.configuration = configuration;
            this.accountName = options.Account;
            this.workspaceName = options.Workspace;
            this.loginCommand = new LoginCommand(this.accountName);
        }

        public override void Run()
        {
            loginCommand.Run();

            Console.WriteLine("Run for account '{0}' and workspace '{1}'", accountName, workspaceName);

            var rootPath = Path.Combine(Environment.CurrentDirectory, this.accountName);
            Console.WriteLine("Watching '{0}' for changes", rootPath);

            var watcher = new Watcher(this.accountName, this.workspaceName, rootPath, configuration);
            watcher.ChangesSent += ChangesSent;
            watcher.RequestFailed += RequestFailed;
            watcher.FileSystemError += exception => Console.Error.WriteLine(exception);

            watcher.Resync();
            watcher.Start();

            Console.WriteLine("Press \'q\' to quit.");
            while (Console.Read() != 'q') ;
        }

        public void ChangesSent(IEnumerable<Change> changes)
        {
            foreach (var change in changes)
            {
                Console.ForegroundColor = change.Action == ChangeAction.Update ? ConsoleColor.Cyan : ConsoleColor.Red;
                Console.Write(change.Action.ToString().ToUpper() + " ");
                Console.ResetColor();
                Console.WriteLine(change.Path);
            }
            Console.WriteLine("waiting for changes...");
        }

        public void RequestFailed(RequestFailedArgs args)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Request failed with status code {0} ({1})", args.StatusCodeText, args.StatusCode);
            Console.ResetColor();
        }
    }
}