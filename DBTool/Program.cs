using DBTool.CLI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBTool
{
    class Program
    {
#if DEBUG
        static void Main(string[] args)
        {
            while (true)
                new Interface(Console.ReadLine().Split(' '));
        }
#else
        static void Main(string[] args)
        {
            try
            {
                new Interface(args);
            }
            catch (Exception e)
            {
                Console.Write(string.Join(Environment.NewLine,string.Empty,e,string.Empty));
            }
        }
#endif
    }
}
