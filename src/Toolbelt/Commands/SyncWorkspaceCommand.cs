using System;
using System.Collections.Generic;
using System.Linq;
using Vtex.Toolbelt.CommandFramework;
using Vtex.Toolbelt.Model;
using Vtex.Toolbelt.Services;

namespace Vtex.Toolbelt.Commands
{
    [Description("watch and synchronize folder with workspace"), Alias("sync")]
    public class SyncWorkspaceCommand : SyncCommand
    {
        private readonly WatcherFactory _factory;
        private string _workspace;

        public SyncWorkspaceCommand(
            IConsole console, IFileSystem fileSystem, LoginService login, WatcherFactory factory)
            : base(console, fileSystem, login)
        {
            _factory = factory;
            OptionSet.Add(0, "workspace", "target workspace", true, value => _workspace = value);
        }

        protected override Watcher GetWatcher(string accountName, string authenticationToken)
        {
            return _factory.CreateAccountWatcher(accountName, _workspace, authenticationToken);
        }

        protected override void NotifyStart(string accountName)
        {
            Console.WriteLine("Using [#magenta {0}] of [#magenta {1}]", _workspace, accountName);
            base.NotifyStart(accountName);
        }

        protected override void BeforeSync(Watcher watcher)
        {
            var accountWatcher = (AccountWatcher) watcher;
            var conflicts = accountWatcher.IdentifyConflicts().ToList();
            if (!conflicts.Any())
                return;

            Console.WriteLine("The following differences were found between the local and remote files:");
            Console.WriteLine();

            WriteConflictsTable(conflicts);

            Console.WriteLine();
            var choice = Console.Prompt("What do you want to do?", options => options
                .Add('D', "Download remote")
                .Add('U', "Upload local")
                .Add('C', "Cancel", true));

            switch (choice)
            {
                case 'D':
                    Console.WriteLine();
                    Console.Write("Fetching remote changes...");
                    accountWatcher.ResolveWithRemote(conflicts);
                    Console.WriteLine(" done!");
                    break;

                case 'U':
                    Console.WriteLine();
                    Console.Write("Sending local changes...");
                    accountWatcher.ResolveWithLocal(conflicts);
                    Console.WriteLine(" done!");
                    break;

                default:
                   throw new ApplicationException("Cancelled by user");
            }
        }

        private void WriteConflictsTable(ICollection<FileConflict> conflicts)
        {
            const int max = 15;
            var table = conflicts.Take(max).OrderBy(conflict => conflict.Path)
                .Select(conflict => Tuple.Create(
                    FileConflict.HumanizeBytes(conflict.LocalSize),
                    FileConflict.HumanizeBytes(conflict.RemoteSize),
                    conflict.Path)).ToList();

            table.Insert(0, Tuple.Create("Local", "Remote", "Path"));

            var widths = new[]
            {
                table.Max(x => x.Item1.Length),
                table.Max(x => x.Item2.Length),
                table.Max(x => x.Item3.Length)
            };

            foreach (var conflict in table)
            {
                Console.WriteLine("| {0} | {1} | {2} |", conflict.Item1.PadRight(widths[0]),
                    conflict.Item2.PadRight(widths[1]), conflict.Item3.PadRight(widths[2]));
            }

            if(conflicts.Count > max)
                Console.WriteLine("And {0} more", conflicts.Count - max);
        }
    }
}