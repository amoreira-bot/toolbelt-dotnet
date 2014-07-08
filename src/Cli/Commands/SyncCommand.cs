using System;
using System.Collections.Generic;
using System.IO;
using Vtex.Toolbelt.Core;
using System.Text;
using Newtonsoft.Json;

namespace Vtex.Toolbelt.Cli.Commands
{
    public class SyncCommand : Command
    {
        private readonly Configuration configuration;
        private readonly string workspaceName;
        private readonly LoginCommand loginCommand;

        public SyncCommand(SyncVerbOptions options, Configuration configuration)
        {
            this.configuration = configuration;
            this.workspaceName = options.Workspace;
            this.loginCommand = new LoginCommand();
        }

        public override void Run()
        {
            var package = this.ReadPackageJson();
            package.Validate();

            var credential = loginCommand.GetValidCredential();

            Console.WriteLine("Run for account '{0}' and workspace '{1}'", package.Name, workspaceName);

            var rootPath = Environment.CurrentDirectory;
            Console.WriteLine("Watching '{0}' for changes", rootPath);

            var watcher = new Watcher(package.Name, this.workspaceName, rootPath, credential.Token, configuration);
            watcher.ChangesSent += ChangesSent;
            watcher.RequestFailed += RequestFailed;
            watcher.FileSystemError += exception => Console.Error.WriteLine(exception);

            watcher.Resync();
            watcher.Start();

            Console.WriteLine("Press \'q\' to quit.");
            while (Console.Read() != 'q') ;
        }

        private PackageJson ReadPackageJson()
        {
            try
            {
                var filePath = Path.Combine(Environment.CurrentDirectory, "package.json");
                var json = File.ReadAllText(filePath, Encoding.UTF8);
                return JsonConvert.DeserializeObject<PackageJson>(json);
            }
            catch (FileNotFoundException)
            {
                throw  new ApplicationException("Couldn't found package.json file.");
            }
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
