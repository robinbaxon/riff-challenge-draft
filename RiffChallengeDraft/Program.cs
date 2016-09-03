using RiffChallengeDraft.Cli;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RiffChallengeDraft.DotNetCore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var draftenabler = new DraftFacilitator();
            draftenabler.StartDraft();
        }
    }
}
