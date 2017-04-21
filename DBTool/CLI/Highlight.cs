using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBTool.CLI
{
    static class Highlight
    {
        public static ConsoleColor Color = ConsoleColor.Cyan;
        public static Extensions.ColorGroundType GroundType = Extensions.ColorGroundType.Foreground;
        public static char SeparatorLeft = '[';
        public static char SeparatorRight = ']';
        public static char Wildcard = '█';
    }
}
