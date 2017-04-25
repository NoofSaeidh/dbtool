using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;
using DBTool.CLI.Options;
using DBTool.CLI.Configuration;
using DBTool.Core;
using System.IO;

namespace DBTool.CLI
{
    class Interface
    {
        public Interface(string[] args)
        {
            var result = Parser.Default.ParseArguments<CreateBackup, RestoreBackup, Dialog>(args);
            var mapped = result.MapResult(option => option,_ => null);
            if (mapped == null) return;

            if (mapped is Dialog) throw new NotImplementedException();
            Config.Initialize(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config.xml"));

            var backup = mapped as IBackup;
            if(backup!=null)
            {
                if (backup.Server == null) backup.Server = Config.Instance.DefaultServer;

                using (var database = new Database(backup.Server, backup.Database))
                {
                    if (backup is CreateBackup)
                        database.CreateBackup(backup.Path);
                    else// if(backup is RestoreBackup)
                        database.RestoreBackup(backup.Path);
                }
            }
        }
    }
}
