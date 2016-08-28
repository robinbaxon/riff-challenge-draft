using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RiffChallengeDraft.Core.Helpers;

namespace RiffChallengeDraft.Core.Entities
{
    public class WeeklyTheme
    {
        
        private Random _randomizer;
        private bool _keepWildcardResult;
        private bool? _isWildcard = null;
        private Random Randomizer        
        {
            get
            {
                _randomizer = _randomizer != null ? _randomizer : new Random();
                return _randomizer;
            }
        }
        public string WildcardChoices
        {
            get
            {
                return String.Join(", ", Enum.GetNames(typeof(Genre))).ReplaceEnd(",", " and ");
            }
        }
        public bool IsThemeWeek
        {
            get
            {
                if (!_keepWildcardResult ||  _isWildcard == null)
                {
                    double value = Randomizer.NextDouble();
                    _isWildcard = value > 0.5;
                }
                return (bool)_isWildcard;
            }
        }

        public WeeklyTheme(bool keepwildcardresult = false)
        {
            _keepWildcardResult = keepwildcardresult;
        }
        public Contestant GetRandomContestant(List<Contestant> contestants)
        {
            {
                return contestants.ElementAt(Randomizer.Next(0, contestants.Count));
            }
        }
        public Genre GetRandomGenre()
        {
            return EnumMethods.RandomEnumValue<Genre>();
        }
        public void ResetWildcardDraft()
        {
            _isWildcard = null;
        }
    }
}
