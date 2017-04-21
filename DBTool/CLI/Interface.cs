using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;
using DBTool.CLI.Options;

namespace DBTool.CLI
{
    class Interface
    {
        public Interface(string[] args)
        {
            var result = Parser.Default.ParseArguments<CreateBackup, RestoreBackup>(args);
        }
    }
}
