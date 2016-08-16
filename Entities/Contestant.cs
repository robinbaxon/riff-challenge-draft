using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace riff_challenge_draft.Entities
{
    public class Contestant
    {
        public string Name { get; set; }
        public Contestant ChallengedBy { get; set; }
        public Contestant ContestantToChallenge { get; set; }
        public Contestant(string name)
        {
            Name = name;
        }
    }
}
