using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;
using DBTool.CLI.Options;
using DBTool.CLI.Configuration;
using System.IO;

namespace DBTool.CLI
{
    class Interface
    {
        public Interface(string[] args)
        {
            var result = Parser.Default.ParseArguments<CreateBackup, RestoreBackup, Dialog>(args);
            var mapped = result.MapResult(option => option,_ => null);
            if (mapped is Dialog) throw new NotImplementedException();
            //(mapped as CreateBackup).
            Config.Initialize(Path.Combine(Environment.CurrentDirectory, "CLI", "Configuration", "Config.xml"));
        }
    }
}
