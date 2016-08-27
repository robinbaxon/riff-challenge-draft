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
        const string WELCOME_TEXT = "Welcome to the Riff Challenge Draft CLI 2000!";
        const string WILDCARD_QUESTION = "Is it wildcard week?";
        const string WILDCARD_YES = "YES!!11";
        const string WILDCARD_NO = "No, sorry. Better luck next week.";
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
            
            UIHelper.WriteLine("Order is determined! Now on to the wildcard week lottery.");
            UIHelper.AwaitUserInput();
            RunWildcardDraft();

            UIHelper.AwaitUserInput();
            UIHelper.WriteLine("Aaaaaaaand, WE'RE DONE! Thanks for tuning in, see you next week!");
            UIHelper.AwaitUserInput();
        }

        private void RunWildcardDraft()
        {
            // Is it wildcard week?
            UIHelper.WriteLine(WILDCARD_QUESTION + ((WeeklyTheme.IsWildcard) ? WILDCARD_YES : WILDCARD_NO));
            if (WeeklyTheme.IsWildcard)
            {
                UIHelper.WriteLine("WILDCARD TIME!");
                UIHelper.WriteLine("What is it going to be? The choices are: " + WeeklyTheme.WildcardChoices);
                UIHelper.AwaitUserInput();
                var choice = WeeklyTheme.GetRandomGenre();
                UIHelper.WriteLine("The wildcard choice is: " + choice.ToString());
                UIHelper.WriteLine(String.Format("Which contestant is going to get his {0} wildcard pick of the week?", choice));
                var wildcardContestant = WeeklyTheme.GetWeeklyThemeSubject(ContestantsDrawn);
                UIHelper.WriteLine(String.Format("It's going to be: {0}! Congratulations.", wildcardContestant.Name), animate: true);
            }
        }

        private void WriteWelcomeText()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            UIHelper.WriteLine(WELCOME_TEXT);
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
                        String.Format("{0} will challenge {1}, and is challenged by {2}",
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
            UIHelper.WriteLine("Now on to the draft.", true);
            Console.ForegroundColor = ConsoleColor.Yellow;
            while (ContestantPool.Any())
            {
                DrawContestant();
            }
            Console.ResetColor();
            UIHelper.WriteLine("All contestants drawn! Moving on... ");
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
                    UIHelper.WriteLine("We're now going to gather information about the contestants. \n");
                }
                UIHelper.WriteLine("");
                UIHelper.Write(String.Format("Name of contestant number {0}: ", ContestantPool.Count + 1));
                var name = Console.ReadLine();
                ContestantPool.Add(new Contestant(name));
                UIHelper.Write(" Another? [Y/N]  ");
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
            UIHelper.WriteLine(String.Format("Contestant drawn: {0}.", ContestantsDrawn.Last().Name), animate: true, speed: UIHelper.WriteSpeed.ExtraSlow);
            UIHelper.AwaitUserInput();
        }
    }
}
