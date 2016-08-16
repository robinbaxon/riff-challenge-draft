using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace riff_challenge_draft.DraftProgram
{
    static class UIHelper
    {
        /// <summary>
        /// Printing a line. Want to improve this somewhat in the future to be a bit more fancy.
        /// </summary>
        /// <param name="str"></param>
        public static void WriteLine(string str)
        {
            Console.WriteLine(str);
        } 

        /// <summary>
        /// Inline printing. Also want to do something fancy with this one at a given point in time. 
        /// </summary>
        /// <param name="str"></param>
        public static void Write(string str)
        {
            Console.Write(str);
        }
    }
}
