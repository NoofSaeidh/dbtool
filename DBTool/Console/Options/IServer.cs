using CommandLine;

namespace DBTool.Console.Options
{
    internal interface IServer
    {
        [Option('s', "server", HelpText = "Database server.")]
        string Server { get; set; }

        [Option('i', "integrated", HelpText = "Integrated security. Use AD account's credentials.")]
        bool IntegratedSecurity { get; set; }

        [Option('u', "username", HelpText = "Password. Used only if Integrated Security is not specified.")]
        string Username { get; set; }

        [Option('p', "password", HelpText = "Password. Used only if Integrated Security is not specified.")]
        string Password { get; set; }

        [Option("con-string", HelpText = "Connection String. If specified, it will be used instead of other options.")]
        string ConnectionString { get; set; }
    }
}