using RiffChallengeDraft.Cli;
using RiffChallengeDraft.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RiffChallengeDraft.Cli
{
    public class DraftFacilitator
    {
        public List<Contestant> ContestantPool;
        public List<Contestant> ContestantsDrawn;
        public WeeklyTheme WeeklyTheme;

        public DraftFacilitator()
        {
            ContestantPool = new List<Contestant>();
            ContestantsDrawn = new List<Contestant>();
            WeeklyTheme = new WeeklyTheme(true);
        }

        public void StartDraft()
        {
            WriteWelcomeText();
            ReadContestantsFromConsole();
            DrawAllContestants();
            AddContestantChallenges();
            PrintContestantsChallengeOrder();
            
            UIHelper.WriteLine(Texts.ORDER_DETERMINED_ON_TO_WILDCARD);
            UIHelper.AwaitUserInput();
            RunWildcardDraft();

            UIHelper.AwaitUserInput();
            UIHelper.WriteLine(Texts.ENDING_TEXT);
            UIHelper.AwaitUserInput();
        }

        private void RunWildcardDraft()
        {
            // Is it wildcard week?
            UIHelper.WriteLine(Texts.WILDCARD_QUESTION);
            UIHelper.WriteLine(((WeeklyTheme.IsWildcard) ? Texts.WILDCARD_YES : Texts.WILDCARD_NO), speed: UIHelper.WriteSpeed.ExtraSlow);
            UIHelper.AwaitUserInput();
            if (WeeklyTheme.IsWildcard)
            {
                UIHelper.WriteLine(Texts.WILDCARD_TIME);
                UIHelper.WriteLine(Texts.WILDCARD_CHOICES_ARE + ": " + WeeklyTheme.WildcardChoices);
                UIHelper.AwaitUserInput();
                var choice = WeeklyTheme.GetRandomGenre();
                
                UIHelper.WriteLine(Texts.WILDCARD_PICK_IS + choice.ToString());
                UIHelper.AwaitUserInput();
                UIHelper.WriteLine(String.Format(Texts.WILDCARD_TEMPLATE_WHICH_CONTESTANT, choice));
                var wildcardContestant = WeeklyTheme.GetWeeklyThemeSubject(ContestantsDrawn);
                UIHelper.WriteLine(String.Format(Texts.WILDCARD_TEMPLATE_PARTICIPANT_PICK, wildcardContestant.Name), animate: true);
            }
        }

        private void WriteWelcomeText()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            UIHelper.WriteLine(Texts.WELCOME_TEXT);
            Console.ResetColor();
        }

        /// <summary>
        /// Print each contestant, who they challenge and who the are challenged by. 
        /// </summary>
        private void PrintContestantsChallengeOrder()
        {
            var contestantsChallengeSummary = new StringBuilder();
            foreach(var contestant in ContestantsDrawn)
            {
                if(contestant.ChallengedBy != null && contestant.ContestantToChallenge != null)
                {
                    contestantsChallengeSummary.AppendLine(
                        String.Format("",
                        contestant.Name, 
                        contestant.ContestantToChallenge.Name, 
                        contestant.ChallengedBy.Name));
                }
            }
            UIHelper.WriteLine(contestantsChallengeSummary.ToString(), animate: true, speed: UIHelper.WriteSpeed.Normal);
            UIHelper.AwaitUserInput();
        }

        /// <summary>
        /// Add contestant challenges
        /// </summary>
        private void AddContestantChallenges()
        {
            foreach(var contestant in ContestantsDrawn)
            {
                var index = ContestantsDrawn.IndexOf(contestant);
                var contestantToBeChallengedBy = (index == 0) ? ContestantsDrawn.Last() : ContestantsDrawn.ElementAt(index - 1); // First one is challenged by last one
                var contestantToChallenge = (index == ContestantsDrawn.Count - 1) ? ContestantsDrawn.First() : ContestantsDrawn.ElementAt(index + 1); // Last one challenges first one
                contestant.ChallengedBy = contestantToBeChallengedBy;
                contestant.ContestantToChallenge = contestantToChallenge;
            }
        }

        /// <summary>
        /// Randomize all contestants to get the draft order.
        /// </summary>
        public void DrawAllContestants()
        {
            UIHelper.WriteLine(Texts.DRAW_ON_TO_DRAFT, true);
            Console.ForegroundColor = ConsoleColor.Yellow;
            while (ContestantPool.Any())
            {
                DrawContestant();
            }
            Console.ResetColor();
            UIHelper.WriteLine(Texts.DRAFT_ALL_DRAWN_MOVING_ON);
            UIHelper.AwaitUserInput();
        }

        /// <summary>
        /// Read all contestants from console, enabling the user to interrupt the registration process after each contestant registration. 
        /// </summary>
        public void ReadContestantsFromConsole()
        {
            bool finished = false;
            while (!finished)
            {
                if (!ContestantPool.Any())
                {
                    UIHelper.WriteLine(Texts.DRAFT_INFORMATION + "\n");
                }
                UIHelper.WriteLine("");
                UIHelper.Write(String.Format("{0} {1}: ", Texts.DRAFT_CONTESTANT_NUMBER, ContestantPool.Count + 1));
                var name = Console.ReadLine();
                ContestantPool.Add(new Contestant(name));
                UIHelper.Write(Texts.DRAFT_ANOTHER + " [y/n]");
                var answer = "";
                while (!Regex.IsMatch(answer, "[ynYN]"))
                {
                    answer = Console.ReadKey(true).Key.ToString();
                    if (answer.ToLower() == "n")
                    {
                        finished = true;
                    }
                }
            }
        }
        public void DrawContestant()
        {
            var number = new Random().Next(0, ContestantPool.Count);
            ContestantsDrawn.Add(ContestantPool.ElementAt(number));
            ContestantPool.RemoveAt(number);
            UIHelper.WriteLine(String.Format("{0}: {1}.",Texts.DRAFT_CONTESTANT_DRAWN, ContestantsDrawn.Last().Name), animate: true, speed: UIHelper.WriteSpeed.ExtraSlow);
            UIHelper.AwaitUserInput();
        }
    }
}
