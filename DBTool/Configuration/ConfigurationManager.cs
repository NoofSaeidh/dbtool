using Config.Net;

namespace DBTool.Configuration
{
    public static class ConfigurationManager
    {
        public static IConfig GetConfig() => new ConfigurationBuilder<IConfig>().UseInMemoryConfig().UseAppConfig().Build();

        public static IConfig GetConfig(string iniFilePath) => new ConfigurationBuilder<IConfig>().UseInMemoryConfig().UseIniFile(iniFilePath).Build();
    }
}