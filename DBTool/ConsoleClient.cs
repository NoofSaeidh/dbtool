using CommandLine;
using DBTool.Configuration;
using DBTool.Console;
using DBTool.Console.Options;
using DBTool.Core;
using System;
using System.Collections.Generic;

namespace DBTool
{
    public static class ConsoleClient
    {
        public static void Execute(IEnumerable<string> args)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));
            var config = new Lazy<IConfig>(() => ConfigurationManager.GetConfig());
            var executor = new Lazy<Executor>(() => new Executor(config.Value.ConnectionString));

            Parser
                .Default
                .ParseArguments<CreateBackup, RestoreBackup>(args)
                .WithParsed<IConfigFile>(cf =>
                {
                    if (!cf.ConfigFile.IsNullOrWhiteSpace())
                        config = new Lazy<IConfig>(() => ConfigurationManager.GetConfig(cf.ConfigFile));
                })
                .WithParsed<IConnectionString>(cs =>
                {
                    if (!cs.ConnectionString.IsNullOrWhiteSpace())
                        config.Value.ConnectionString = cs.ConnectionString;
                })
                .WithParsed<IConfiguration>(cfg =>
                {
                    if (cfg.PreferTempRestoreFiles.HasValue)
                    {
                        config.Value.PreferTempRestoreFiles = cfg.PreferTempRestoreFiles.Value;
                    }
                    if (!cfg.TempFolder.IsNullOrWhiteSpace())
                    {
                        config.Value.TempFolder = cfg.TempFolder;
                    }
                })
                .WithParsed<CreateBackup>(b => TryCatchConsole("Create Backup", () => executor.Value.CreateBackup(b.Database, b.Path)))
                .WithParsed<RestoreBackup>(b => TryCatchConsole("Restore backup", () =>
                        {
                            if (config.Value.PreferTempRestoreFiles)
                            {
                                executor.Value.RestoreBackupWithTempFile(b.Database, b.Path, config.Value.TempFolder);
                            }
                            else
                            {
                                executor.Value.RestoreBackup(b.Database, b.Path);
                            }
                        }));
        }

        private static void TryCatchConsole(string description, Action action)
        {
            try
            {
                System.Console.WriteLine($"Operation \"{description}\" started.");
                action();
                System.Console.WriteLine();
                System.Console.WriteLine($"Operation \"{description}\" successfully completed.");

            }
            catch (Exception e)
            {
                var color = System.Console.ForegroundColor;
                System.Console.ForegroundColor = ConsoleColor.Red;
                System.Console.WriteLine($"Operation \"{description}\" failed.");
                System.Console.WriteLine();
                System.Console.WriteLine(e);
                System.Console.ForegroundColor = color;
            }
            System.Console.WriteLine();
        }
    }
}