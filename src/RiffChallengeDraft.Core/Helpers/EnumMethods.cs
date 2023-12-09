using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiffChallengeDraft.Core.Helpers
{
    static class EnumMethods
    {
        /// <summary>
        /// Returns random enum value of given enum
        /// Shamelessly stolen from mafu's generic approach answer to http://stackoverflow.com/questions/3132126/how-do-i-select-a-random-value-from-an-enumeration 
        /// </summary>
        /// <typeparam name="T">An enum of sorts</typeparam>
        /// <returns></returns>
        public static T RandomEnumValue<T>()
        {
            var v = Enum.GetValues(typeof(T));
            return (T)v.GetValue(new Random().Next(v.Length));
        }
    }
}
