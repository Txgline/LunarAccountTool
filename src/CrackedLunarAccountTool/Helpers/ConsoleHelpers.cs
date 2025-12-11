using System;
using System.Drawing;
using Console = Colorful.Console;

namespace LunarAccountTool.Helpers
{
    internal class ConsoleHelpers
    {
        public static void Print(string info, string text, Color color)
        {
            var dateDebug = DateTime.Now.ToString("HH:mm:ss");

            // Print the timestamp
            Console.Write($@" [{dateDebug}] > ", Color.FromArgb(80, 80, 80));

            // Only print [info] if it's not empty
            if (!string.IsNullOrEmpty(info))
            {
                Console.Write($@"[{info}] ", color);
            }

            Console.Write(text);
        }

        public static void PrintLine(string info, string text, Color color)
        {
            // Print the timestamp
            Console.Write("[", Color.Gray);
            Console.Write(DateTime.Now.ToString("HH:mm:ss"), Color.DarkGray);
            Console.Write("] ", Color.Gray);

            // Only print [info] if it's not empty
            if (!string.IsNullOrEmpty(info))
            {
                Console.Write($"[{info}] ", color);
            }

            Console.WriteLine(text, Color.White);
        }
    }
}
