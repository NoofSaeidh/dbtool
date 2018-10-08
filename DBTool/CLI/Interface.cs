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
            ScriptExecutor scriptExecutor = null;
            Config.Initialize(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config.xml"));
            var result = Parser.Default.ParseArguments<CreateBackup, RestoreBackup>(args);
            result.WithParsed<IDatabase>(db =>
            {
                if (db.IntegratedSecurity)
                {
                    scriptExecutor = new ScriptExecutor(db.Server ?? Config.Instance.DefaultServer);
                }
                else
                {
                    scriptExecutor = new ScriptExecutor(db.Server ?? Config.Instance.DefaultServer, db.Username, db.Password);
                }
            });
            var mapped = result.MapResult<CreateBackup, RestoreBackup, Exception>(
                create => TryCatch(() => scriptExecutor.CreateBackup(create.Database, create.Path)),
                restore => TryCatch(() => scriptExecutor.RestoreBackup(restore.Database, restore.Path)),
                errors => null
            );

            if(mapped != null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine(mapped);
            }
        }
        private Exception TryCatch(Action action)
        {
            try
            {
                action();
                return null;
            }
            catch (Exception e)
            {
                return e;
            }
        }
    }
}
