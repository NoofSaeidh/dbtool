namespace DBTool.Configuration
{
    public interface IConfig
    {
        string ConnectionString { get; set; }
        bool PreferTempRestoreFiles { get; set; }
        string TempFolder { get; set; }
    }
}