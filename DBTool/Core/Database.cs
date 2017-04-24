using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBTool.Core
{
    public class Database : IDisposable
    {
        #region Fields
        private SqlCommand command;
        private bool newVersion;
        private string _version;
        private const int defaultTimeout = 30000;
        #endregion

        #region Properties
        public string Version
        {
            get
            {
                return _version;
            }

            private set
            {
                _version = value;
                if (int.Parse(_version.Split('.').First()) > 12)
                    newVersion = true;
                else newVersion = false;
            }
        }
        public string Server { get; set; }
        public string Name { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public bool IntegratedSecurity { get; set; } = true;
        public int? Timeout
        {
            get { return command?.CommandTimeout; }
            set { command.CommandTimeout = value.GetValueOrDefault(); }
        }
        #endregion

        #region Init
        //If use empty constructor - use Connect
        public Database(string server, string database
            , bool integratedSecurity = true
            , string user = null, string password = null)
            : this()
        {
            Server = server;
            Name = database;
            IntegratedSecurity = integratedSecurity;
            User = user;
            Password = Password;
            Connect(false);
        }
        public Database()
        {
            command = new SqlCommand()
            {
                CommandTimeout = defaultTimeout,
            };
        }
        public void Connect(bool master = false)
        {
            command.Connection = CreateConnection(master);
            Version = FindOutVersion();
        }

        void IDisposable.Dispose()
        {
            command.Dispose();
        }
        #endregion

        #region Methods
        public string FindOutVersion()
        {
            var version = command
                .Open()
                .Script(Scripts.GetVersion)
                .ExecuteScalar(true)
                .ToString();
            return version;
        }
        public void CreateBackup(string fileName)
        {
            command
                .Open()
                .ExecuteScript(Scripts.BackupToDisk(Name, fileName)
                    , close: true);

        }
        public void RestoreBackup(string fileName)
        {
            command.Open();

            command.ExecuteScript(Scripts.Use("master"));

            command.ExecuteScript(Scripts.AlterToSingle(Name));
            try
            {

                var mdf = command.ExecuteQuery(Scripts.GetMdf(Name)).ToString();

                var ldf = command.ExecuteQuery(Scripts.GetLdf(Name)).ToString();

                var restore = command.ExecuteQuery(Scripts.GetRestoreScript(
                                                                        Name,
                                                                        fileName,
                                                                        mdf,
                                                                        ldf,
                                                                        newVersion))
                                        .ToString();

                command.ExecuteScript(restore);
            }
            catch (Exception e)
            {
                command.ExecuteScript(Scripts.AlterToMulti(Name));
                throw e;
            }

            command.ExecuteScript(Scripts.AlterToMulti(Name));
        }

        protected SqlConnection CreateConnection(bool master = false)
        {
            string connection = $"Data Source={Server}";
            if (master)
                connection += $";Initial Catalog=master;Database={Name}";
            else
                connection += $";Initial Catalog={Name}";
            if (IntegratedSecurity)
                connection += $";Integrated Security=True";
            else
                connection += $";Integrated Security=False;User ID={User};Password={Password}";


            return new SqlConnection(connection);
        }
        #endregion
    }
}
