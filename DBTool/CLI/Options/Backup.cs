using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBTool.CLI.Messages;

namespace DBTool.CLI.Options
{
    interface IBackup
    {
        [Option('S',"server",HelpText = Help.Server)]
        string Server { get; set; }

        [Value(0,HelpText = Help.Database,Required = true)]
        string Database { get; set; }

        [Value(1, HelpText = Help.Path, Required = true)]
        string Path { get; set; }
    }
    [Verb("create",HelpText = Help.Create)]
    class CreateBackup : IBackup
    {
        public string Server { get; set; }

        public string Database { get; set; }

        public string Path { get; set; }
    }
    [Verb("restore", HelpText = Help.Create)]
    class RestoreBackup : IBackup
    {
        public string Server { get; set; }

        public string Database { get; set; }

        public string Path { get; set; }
    }
}
