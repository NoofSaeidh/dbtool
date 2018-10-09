namespace DBTool
{
    internal class Program
    {
#if DEBUG

        private static void Main(string[] args)
        {
            while (true)
                ConsoleClient.Execute(System.Console.ReadLine().Split(' '));
        }

#else
        static void Main(string[] args)
        {
            ConsoleClient.Execute(args);
        }
#endif
    }
}