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
                if(ConsoleColors.ChosenOnes == null)
                {
                    ConsoleColors.ChosenOnes = new List<ConsoleColor>();
                }
                if(_color == null)
                {
                    while(_color == null || _color == Console.BackgroundColor || ConsoleColors.ChosenOnes.Contains((ConsoleColor)_color))
                    {
                        _color = EnumMethods.RandomEnumValue<ConsoleColor>();
                    }
                    ConsoleColors.ChosenOnes.Add((ConsoleColor)_color);
                    if(ConsoleColors.ChosenOnes.Count >= 15)
                    {
                        // There are only 16 console colors, and we've already eliminated the background color. Have to reset colors.
                        ConsoleColors.ChosenOnes = new List<ConsoleColor>();
                    }
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

    public static class ConsoleColors
    {
        public static List<ConsoleColor> ChosenOnes;
    }
}
