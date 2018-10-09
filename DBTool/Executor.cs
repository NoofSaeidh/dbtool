using DBTool.Core;
using System;
using System.IO;

namespace DBTool
{
    public class Executor
    {
        private readonly ScriptExecutor _scriptExecutor;

        public Executor(string connectionString)
        {
            _scriptExecutor = new ScriptExecutor(connectionString);
        }

        public void CreateBackup(string database, string file)
        {
            _scriptExecutor.CreateBackup(database, file);
        }

        public void RestoreBackupWithTempFile(string database, string file, string tempFolder)
        {
            string resPath;
            var tempName = Guid.NewGuid().ToString() + ".bak";
            if (tempFolder.IsNullOrWhiteSpace())
            {
                resPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, tempName);
            }
            else
            {
                if (!Directory.Exists(tempFolder))
                {
                    Directory.CreateDirectory(tempFolder);
                }
                resPath = Path.Combine(tempFolder, tempName);
            }
            File.Copy(file, resPath, true);
            try
            {
                RestoreBackup(database, resPath);
            }
            finally
            {
                File.Delete(resPath);
            }
        }

        public void RestoreBackup(string database, string file)
        {
            _scriptExecutor.RestoreBackup(database, file);
        }
    }
}