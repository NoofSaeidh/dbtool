using CommandLine;
using DBTool.Core;

namespace DBTool.Console.Options
{
    internal abstract class BackupBase : IServer, IConfigFile, IConnectionString, IConfiguration
    {
        public string Server { get; set; }
        public bool IntegratedSecurity { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ConnectionString { get; set; }
        public string ConfigFile { get; set; }
        public bool? PreferTempRestoreFiles { get; set; }
        public string TempFolder { get; set; }

        [Value(0, HelpText = "Database name.", Required = true)]
        public string Database { get; set; }

        [Value(1, HelpText = "Full path to file.", Required = true)]
        public string Path { get; set; }


        string IConnectionString.ConnectionString
        {
            get
            {
                if (!ConnectionString.IsNullOrWhiteSpace())
                    return ConnectionString;
                if (!Server.IsNullOrWhiteSpace())
                    return ScriptExecutor.GetConnectionString(Server, IntegratedSecurity ? true : Username.IsNullOrEmpty() ? true : false, Username, Password);
                return null;
            }
        }
    }

    [Verb("create", HelpText = "Create backup.")]
    internal class CreateBackup : BackupBase
    {
    }

    [Verb("restore", HelpText = "Restore backup.")]
    internal class RestoreBackup : BackupBase
    {
    }
}