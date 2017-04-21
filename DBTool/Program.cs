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
        static void Main(string[] args)
        {
            while (true) 
                new Interface(Console.ReadLine().Split(' '));
        }
    }
}
