using System;

namespace DBTool.Console
{
    internal static class Highlight
    {
        public static ConsoleColor Color = ConsoleColor.Cyan;
        public static Extensions.ColorGroundType GroundType = Extensions.ColorGroundType.Foreground;
        public static char SeparatorLeft = '[';
        public static char SeparatorRight = ']';
        public static char Wildcard = '█';
    }
}