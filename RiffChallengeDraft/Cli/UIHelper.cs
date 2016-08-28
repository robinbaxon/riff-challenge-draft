using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiffChallengeDraft.Cli
{
    static class UIHelper
    {
        static volatile bool userInterrupt = false;

        public enum WriteSpeed { Fast = 10, Normal = 40, Slow = 70, ExtraSlow = 110};
        /// <summary>
        /// Printing a line. Want to improve this somewhat in the future to be a bit more fancy.
        /// </summary>
        /// <param name="str"></param>
        public static void WriteLine(string str, bool printextraline = false, bool animate = true, WriteSpeed speed = WriteSpeed.Fast)
        {
            if (printextraline)
            {
                Console.WriteLine("");
            }
            if (animate)
            {
                Write(str, speed, true, true);
            }
            else
            {
                Console.WriteLine(str);
            }
        }

        /// <summary>
        /// Inline printing. Also want to do something fancy with this one at a given point in time. 
        /// </summary>
        /// <param name="str"></param>
        public static void Write(string str, WriteSpeed speed = WriteSpeed.Normal, bool addline = false, bool animate = false)
        {
            if (animate)
            {
                foreach (var c in str)
                {
                    Console.Write(c);
                    System.Threading.Thread.Sleep((int)speed);
                }
            }
            else
            {
                Console.Write(str);
            }
            
            if (addline)
            {
                Console.WriteLine();
            }
        }

        public static void AwaitUserInput()
        {
            Task.Factory.StartNew(() =>
            {
                while (Console.ReadKey().Key != ConsoleKey.Enter) ;
                userInterrupt = true;
            });

            while (!userInterrupt)
            {
                ConsoleSpinner.Turn();
                System.Threading.Thread.Sleep(120);
            }
            userInterrupt = false;
        }
    }
}
