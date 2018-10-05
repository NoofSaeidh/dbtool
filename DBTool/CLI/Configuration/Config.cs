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
                switch (record.Name.ToString())
                {
                    case nameof(CreateBackup):
                        backup = new CreateBackup();
                        break;
                    case nameof(RestoreBackup):
                        backup = new RestoreBackup();
                        break;
                    default:
                        throw new System.Xml.XmlException(Text.XmlNotCorrect);
                }

                backup.Database = record.Attribute(nameof(backup.Database))?.Value;
                backup.Path = record.Attribute(nameof(backup.Path))?.Value;
                backup.Server = record.Attribute(nameof(backup.Server))?.Value;
                config.Records.Add(backup);
            }

            return config;
        }

        protected static string Deparse(Config config)
        {
            var xdoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"));
            xdoc.Add(new XElement("Config",
                        new XElement("Default"),
                        new XElement("Records")));
            if (config.DefaultServer != null)
                xdoc.Descendants("Default").First().Add(new XAttribute("Server", config.DefaultServer));

            if (config.Records?.Count > 0)
            {
                foreach (var record in config.Records)
                {
                    var backup = record is RestoreBackup ?
                                    "RestoreBackup" : //record is CreateBackup ?
                                    "CreateBackup"; //: null;
                    xdoc.Descendants("Records").First().Add(new XElement(backup));
                    var element = xdoc.Descendants(backup).Last();
                    if (record.Database != null) element.Add(new XAttribute("Database", record.Database));
                    if (record.Path != null) element.Add(new XAttribute("Path", record.Path));
                    if (record.Server != null) element.Add(new XAttribute("Server", record.Server));
                }
            }
            return string.Concat(xdoc.Declaration.ToString(),Environment.NewLine, xdoc.ToString());
        }

        #endregion

        #region Public Methods

        public void Save()
        {
            Deparse(this);
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
