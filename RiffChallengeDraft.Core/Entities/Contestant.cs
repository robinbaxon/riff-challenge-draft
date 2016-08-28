using RiffChallengeDraft.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiffChallengeDraft.Core.Entities
{
    public class Contestant
    {
        private ConsoleColor? _color;
        public ConsoleColor Color
        {
            get
            {
                if(_color == null)
                {
                    _color = EnumMethods.RandomEnumValue<ConsoleColor>();
                }
                return (ConsoleColor)_color;
            }
        }
        public string Name { get; set; }
        public Contestant ChallengedBy { get; set; }
        public Contestant ContestantToChallenge { get; set; }

        public override string ToString()
        {
            return !String.IsNullOrEmpty(Name) ? Name : base.ToString();
        }
    }
}
