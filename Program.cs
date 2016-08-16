using riff_challenge_draft.DraftProgram;
using riff_challenge_draft.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace riff_challenge_draft
{
    class Program
    {
        static void Main(string[] args)
        {
            var draftenabler = new DraftEnabler();
            draftenabler.StartDraft();
        }
    }
}
