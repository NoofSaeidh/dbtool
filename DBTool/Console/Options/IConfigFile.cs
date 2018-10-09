using CommandLine;

namespace DBTool.Console.Options
{
    public interface IConfigFile
    {
        [Option("config", HelpText = "Path to custom ini config.")]
        string ConfigFile { get; set; }
    }
}