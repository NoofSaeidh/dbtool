using System;
using System.Data.SqlClient;
using System.Linq;

namespace DBTool.Core
{
    public class ScriptExecutor : IDisposable
    {
        #region Fields
        public const int DefaultTimeout = 30000;
        public const string MasterDatabase = "[master]";
        private SqlConnection _connection;
        private bool? _newVersion;
        private string _version;
        #endregion

        #region Init

        public ScriptExecutor(string server, bool integratedSecurity, string user, string password)
        {
            Server = server;
            IntegratedSecurity = integratedSecurity;
            User = user;
            Password = password;
            _connection = CreateConnection();
            _connection.Open();
        }

        public ScriptExecutor(string server, string user, string password) : this(server, false, user, password)
        {
        }

        public ScriptExecutor(string server) : this(server,  true, null, null)
        {
        }

        void IDisposable.Dispose()
        {
            _connection.Dispose();
        }

        #endregion

        #region Properties

        public bool IntegratedSecurity { get; }

        public string Password { get; }

        public string Server { get; }

        public int? Timeout { get; set; }

        public string User { get; }

        public string Version
        {
            get => _version ?? (_version = GetVersion());
            private set => _version = value;
        }

        private bool NewVersion => _newVersion ?? (bool)(_newVersion = (int.Parse(Version.Split('.').First()) > 12) ? true : false);
        #endregion


        #region Methods

        public void CreateBackup(string database, string fileName)
        {
            ExecuteNonQuery(Scripts.BackupToDisk(database, fileName));
        }

        public string GetVersion()
        {
            return ExecuteScalar<string>(Scripts.GetVersion);
        }

        public void RestoreBackup(string database, string fileName)
        {
            using (var command = GetCommand())
            {
                command.ExecuteNonQuery(Scripts.Join(Scripts.Use(MasterDatabase), Scripts.AlterToSingle(database)));

                try
                {
                    var mdf = command.ExecuteScalar<string>(Scripts.GetMdf(database));
                    var ldf = command.ExecuteScalar<string>(Scripts.GetLdf(database));
                    var restoreScript = command.ExecuteScalar<string>(Scripts.GetRestoreScript(database, fileName, mdf, ldf, NewVersion));
                    command.ExecuteNonQuery(restoreScript, true);
                }
                catch(Exception e)
                {
                    throw new DatabaseExecutionException("Could not restore backup.", e);
                }
                finally
                {
                    try
                    {
                        command.ExecuteNonQuery(Scripts.AlterToMulti(database));
                    }
                    catch { }
                }
            }

        }

        protected SqlConnection CreateConnection()
        {
            var connection = $"Data Source={Server};Initial Catalog=master;";
            if (IntegratedSecurity)
                connection += $"Integrated Security=True;";
            else
                connection += $"Integrated Security=False;User ID={User};Password={Password};";

            return new SqlConnection(connection);
        }

        protected int ExecuteNonQuery(string cmdText, bool checkAnyRowAffected = false) => GetCommand().ExecuteNonQueryDispose(cmdText, checkAnyRowAffected);

        protected SqlDataReader ExecuteReader(string cmdText) => GetCommand().ExecuteReaderDispose(cmdText);

        protected T ExecuteScalar<T>(string cmdText) => GetCommand().ExecuteScalarDispose<T>(cmdText);

        protected object ExecuteScalar(string cmdText) => GetCommand().ExecuteScalarDispose(cmdText);

        protected SqlCommand GetCommand()
        {
            return new SqlCommand
            {
                Connection = _connection,
                CommandTimeout = Timeout ?? DefaultTimeout
            };
        }
        #endregion
    }
}