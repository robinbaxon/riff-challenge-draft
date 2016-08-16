using riff_challenge_draft.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace riff_challenge_draft.DraftProgram
{
    public class DraftEnabler
    {
        private List<Contestant> ContestantPool;
        private List<Contestant> ContestantsDrawn;
        private WeeklyTheme WeeklyTheme; 

        public DraftEnabler()
        {
            ContestantPool = new List<Contestant>();
            ContestantsDrawn = new List<Contestant>();
            WeeklyTheme = new WeeklyTheme();
        }

        public void StartDraft()
        {
            
            ReadContestantsFromConsole();
            DrawAllContestants();
            AddContestantChallenges();

            
            PrintContestantsChallengeOrder();

            Console.WriteLine("Order is determined! Now on to the wildcard week lottery. Press any key to continue.");
            Console.ReadKey();
            // Is it wildcard week?
            UIHelper.WriteLine("Is it wildcard week? " + ((WeeklyTheme.IsWildcard) ? "YES!!!1" : "no. sry"));
            Console.WriteLine("And we're done! Thanks for tuning in, see you next week!");
            Console.ReadKey();
            
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
            UIHelper.Write(contestantsChallengeSummary.ToString());
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
            while (ContestantPool.Any())
            {
                DrawContestant();
            }
            UIHelper.WriteLine("All contestants drawn! Moving on. ");
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
                    UIHelper.WriteLine("We're now going to gather information about the contestants. Please start by entering the name of the first contestant. \n");
                }
                UIHelper.WriteLine("");
                UIHelper.WriteLine(String.Format("Please write the name of contestant number {0}: ", ContestantPool.Count + 1));
                var name = Console.ReadLine();
                ContestantPool.Add(new Contestant(name));
                UIHelper.WriteLine("");
                UIHelper.Write("Great, thanks. Do you want to add another one? [Y/N]  ");
                var answer = "";
                while (!Regex.IsMatch(answer, "[ynYN]"))
                {
                    answer = Console.ReadKey().Key.ToString();
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
            UIHelper.WriteLine(String.Format("Contestant drawn: {0}. Press any key to draw next.", ContestantsDrawn.Last().Name));
            UIHelper.WriteLine("");
            Console.ReadKey();
        }
    }
}
