using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RiffChallengeDraft.Core.ExtensionMethods
{
    public static class StringExtensions
    {
        public static string RegexReplace(this string source, string pattern, string replacement)
        {
            return Regex.Replace(source, pattern, replacement);
        }

        public static string ReplaceEnd(this string source, string value, string replacement)
        {
            return RegexReplace(source, $"{value}$", replacement);
        }
    }
}
