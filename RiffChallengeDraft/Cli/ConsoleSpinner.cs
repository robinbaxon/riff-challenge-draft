using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiffChallengeDraft.Cli
{
    /// <summary>
    /// Shamelessly stolen from Daniel Ballingers answer to this StackOverflow question: http://stackoverflow.com/questions/1923323/console-animations
    /// </summary>
    static class ConsoleSpinner
    {
        static int counter;
        public static void Turn()
        {
            counter++;
            switch (counter % 4)
            {
                case 0: Console.Write("/"); break;
                case 1: Console.Write("-"); break;
                case 2: Console.Write("\\"); break;
                case 3: Console.Write("|"); break;
            }
            Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
        }
    }
}
