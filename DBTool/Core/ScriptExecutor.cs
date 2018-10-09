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

        public ScriptExecutor(string connectionString)
        {
            if (connectionString.IsNullOrWhiteSpace())
                throw new ArgumentException("Connection string must not be null or empty", nameof(connectionString));

            _connection = new SqlConnection(connectionString);
            _connection.Open();
        }

        public ScriptExecutor(string server, bool integratedSecurity, string userid, string password) : this(GetConnectionString(server, integratedSecurity, userid, password))
        {
        }

        public static ScriptExecutor FromCredentials(string server, string userid, string password)
        {
            return new ScriptExecutor(server, false, userid, password);
        }

        public static ScriptExecutor FromIntegratedSecurity(string server)
        {
            return new ScriptExecutor(server, true, null, null);
        }

        void IDisposable.Dispose()
        {
            _connection.Dispose();
        }

        #endregion

        #region Properties

        public string ConnectionString { get; }

        public bool IntegratedSecurity => throw new NotImplementedException();

        public string Password => throw new NotImplementedException();

        public string Server => throw new NotImplementedException();

        public int? Timeout { get; set; }

        public string User => throw new NotImplementedException();

        public string Version
        {
            get
            {
                if (_version != null)
                    return _version;
                try
                {
                    return _version = ExecuteScalar<string>(Scripts.GetVersion);
                }
                catch (Exception e)
                {
                    throw new DatabaseExecutionException("Could not get version.", e);
                }
            }

            private set => _version = value;
        }

        private bool NewVersion => _newVersion ?? (bool)(_newVersion = (int.Parse(Version.Split('.').First()) > 12) ? true : false);

        #endregion

        #region Methods Public

        public void CreateBackup(string database, string fileName)
        {
            try
            {
                ExecuteNonQuery(Scripts.BackupToDisk(database, fileName));
            }
            catch (Exception e)
            {
                throw new DatabaseExecutionException("Could not create backup.", e);
            }
        }

        public void RestoreBackup(string database, string fileName)
        {
            using (var command = GetCommand())
            {
                try
                {
                    command.ExecuteNonQuery(Scripts.AlterToSingle(database));
                }
                catch (SqlException se) when (se.Message.Contains("The database is in single-user mode, and a user is currently connected to it."))
                {
                    // already but another user connected.
                    // todo: kill process using this database
                    throw new DatabaseExecutionException("Could not get access to database. The database is in single-user mode, and a user is currently connected to it.");
                }
                try
                {
                    var mdf = command.ExecuteScalar<string>(Scripts.GetMdf(database));
                    var ldf = command.ExecuteScalar<string>(Scripts.GetLdf(database));
                    var restoreScript = command.ExecuteScalar<string>(Scripts.GetRestoreScript(database, fileName, mdf, ldf, NewVersion));
                    command.ExecuteNonQuery(restoreScript);
                }
                catch (Exception e)
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

        #endregion

        #region Methods Protected

        public static string GetConnectionString(string server, bool integratedSecuity, string userid, string password)
        {
            if (server == null)
                throw new ArgumentNullException(nameof(server));

            return $"Data Source={server};Integrated Security={integratedSecuity};"
                + (integratedSecuity
                    ? null
                    : $"User ID={userid};Password={password}");
        }

        protected SqlCommand GetCommand()
        {
            return new SqlCommand
            {
                Connection = _connection,
                CommandTimeout = Timeout ?? DefaultTimeout
            };
        }

        protected int ExecuteNonQuery(string cmdText, bool checkAnyRowAffected = false) => GetCommand().ExecuteNonQueryDispose(cmdText, checkAnyRowAffected);

        protected SqlDataReader ExecuteReader(string cmdText) => GetCommand().ExecuteReaderDispose(cmdText);

        protected T ExecuteScalar<T>(string cmdText) => GetCommand().ExecuteScalarDispose<T>(cmdText);

        protected object ExecuteScalar(string cmdText) => GetCommand().ExecuteScalarDispose(cmdText);

        #endregion
    }
}