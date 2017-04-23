using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBTool.CLI.Options;
using System.Xml.Linq;
using System.IO;
using DBTool.CLI.Messages;

namespace DBTool.CLI.Configuration
{
    class Config
    {
        #region Properties

        public List<IBackup> Records { get; set; }

        public string DefaultServer { get; set; }

        public string ConfigPath { get; set; }

        public static Config Instance { get; private set; }

        #endregion


        #region Parser 

        protected static Config Parse(string xmlPath)
        {
            var config = new Config();
            var xdoc = XDocument.Load(xmlPath);

            var defaults = xdoc.Descendants("Default");
            config.DefaultServer = defaults.Attributes("Server").FirstOrDefault()?.Value;

            var records = xdoc.Descendants("Records");
            config.Records = new List<IBackup>();
            foreach (var record in records.Descendants())
            {
                IBackup backup;
                if (record.Name == nameof(CreateBackup))
                    backup = new CreateBackup();
                else if (record.Name == nameof(RestoreBackup))
                    backup = new RestoreBackup();
                else throw new System.Xml.XmlException(Text.XmlNotCorrect);

                backup.Database = record.Attribute(nameof(backup.Database))?.Value;
                backup.Path = record.Attribute(nameof(backup.Path))?.Value;
                backup.Server = record.Attribute(nameof(backup.Server))?.Value;
                config.Records.Add(backup);
            }

            return config;
        }

        protected static string Deparse(Config config)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Public Methods

        public void Save()
        {
            throw new NotImplementedException();
        }

        #endregion


        #region Initialization

        private Config()
        {

        }

        public static void Initialize(string path)
        {
            Instance = Parse(path);
            Instance.ConfigPath = path;
        }

        #endregion


    }
}
