using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;

namespace DBTool.CLI
{
    class Interface
    {
        public Interface(string[] args)
        {

        }

        class Options
        {
            [Option('i',"interactive", Default = false, HelpText = Messages.InteractiveHelp)]
            public bool Interactive { get; set; }

            [Option('S', "server", HelpText = Messages.ServerHelp)]
            public string Server { get; set; }

            [Option('d', "database", HelpText = Messages.DatabaseHelp)]
            public string Database { get; set; }
        }

        class Messages
        {
            public const string InteractiveHelp = "Interactive mode. Config file is required. Could be usefull, for remembered values.";
            public const string ServerHelp = "Database Provider. If not specified will be used default value from config.";
            public const string DatabaseHelp = "Database name for manipulations.";

        }
    }
}
