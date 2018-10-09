using CommandLine;

namespace DBTool.Console.Options
{
    public interface IConfiguration
    {
        [Option("temp-file", HelpText = "Create temp file before restoring backup.")]
        bool? PreferTempRestoreFiles { get; set; }

        [Option("temp-folder", HelpText = "Folder for creating temp files.")]
        string TempFolder { get; set; }
    }
}