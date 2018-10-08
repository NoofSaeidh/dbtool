using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBTool.CLI.Messages;

namespace DBTool.CLI.Options
{
    interface IDatabase
    {
        [Option('s',"server",HelpText = Help.Server)]
        string Server { get; set; }

        //todo: specifiable
        bool IntegratedSecurity { get; }// set; 

        //todo: helptext
        [Option('u', "username")]
        string Username { get; set; }

        [Option('p', "password")]
        string Password { get; set; }

        [Value(0,HelpText = Help.Database,Required = true)]
        string Database { get; set; }

        [Value(1, HelpText = Help.Path, Required = true)]
        string Path { get; set; }
    }

    [Verb("create",HelpText = Help.Create)]
    class CreateBackup : IDatabase
    {
        public string Server { get; set; }
        public string Database { get; set; }
        public string Path { get; set; }
        public bool IntegratedSecurity => Username == null || Password == null; // todo: make configurable (and use config)
        public string Username { get; set; }
        public string Password { get; set; }
    }

    [Verb("restore", HelpText = Help.Restore)]
    class RestoreBackup : IDatabase
    {
        public string Server { get; set; }
        public string Database { get; set; }
        public string Path { get; set; }
        public bool IntegratedSecurity => Username == null || Password == null; // todo: make configurable (and use config)
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
