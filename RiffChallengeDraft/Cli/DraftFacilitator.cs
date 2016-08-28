using RiffChallengeDraft.Cli;
using RiffChallengeDraft.Core.Entities;
using RiffChallengeDraft.Core.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace RiffChallengeDraft.Cli
{
    public class DraftFacilitator
    {
        
        private Contestant _weeklyWildcardContestant = null;
        private Genre? _weeklyWildcardGenre = null;
        private DateTime _initiatedDateTime;

        public List<Contestant> ContestantsDrawn;
        public List<Contestant> ContestantPool;
        public WeeklyTheme WeeklyTheme;

        public bool PrintVerboseChallengeOrder = false;

        public DraftFacilitator()
        {
            ContestantPool = new List<Contestant>();
            ContestantsDrawn = new List<Contestant>();
            WeeklyTheme = new WeeklyTheme(true);
            _initiatedDateTime = DateTime.Now;
        }

        public void StartDraft()
        {
            LogResultsHeadline(Texts.WELCOME_TEXT);
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
            LogResultsHeadline(Texts.WILDCARD_TIME);
            // Is it wildcard week?
            UIHelper.WriteLine(Texts.WILDCARD_QUESTION);
            UIHelper.AwaitUserInput();
            UIHelper.WriteLine(((WeeklyTheme.IsWildcard) ? Texts.WILDCARD_YES : Texts.WILDCARD_NO), speed: UIHelper.WriteSpeed.ExtraSlow);
            LogResults("Wildcard: " + WeeklyTheme.IsWildcard);
            if (WeeklyTheme.IsWildcard)
            {
                UIHelper.WriteLine(Texts.WILDCARD_TIME);
                UIHelper.WriteLine(Texts.WILDCARD_CHOICES_ARE + ": " + WeeklyTheme.WildcardChoices);
                UIHelper.AwaitUserInput();
                _weeklyWildcardGenre = WeeklyTheme.GetRandomGenre();
                LogResults("Theme type: " + _weeklyWildcardGenre.ToString());
                UIHelper.WriteLine(Texts.WILDCARD_PICK_IS + _weeklyWildcardGenre.ToString());
                UIHelper.AwaitUserInput();
                UIHelper.WriteLine(String.Format(Texts.WILDCARD_TEMPLATE_WHICH_CONTESTANT, _weeklyWildcardGenre));
                _weeklyWildcardContestant = WeeklyTheme.GetWeeklyThemeSubject(ContestantsDrawn);
                UIHelper.WriteLine(String.Format(Texts.WILDCARD_TEMPLATE_PARTICIPANT_PICK, _weeklyWildcardContestant.Name), animate: true);
                LogResults("Pick theme from: " + _weeklyWildcardContestant.Name);
                
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
            LogResultsHeadline("Contestants challenge order and overview");

            if (PrintVerboseChallengeOrder)
            {
                PrintContestantsVerbose();
            }

            /// String table, for nice log outputs
            var contestantsChallengeTable = ContestantsDrawn.ToStringTable(
                cont => cont.Name,
                cont => cont.ChallengedBy,
                cont => cont.ContestantToChallenge
                );
            LogResults(contestantsChallengeTable);
            UIHelper.WriteLine(contestantsChallengeTable, animate: true, speed: UIHelper.WriteSpeed.Fast);
            UIHelper.AwaitUserInput();
        }

        /// <summary>
        /// Older, verbose method of printing the contestants list. Keeping because the author has hoarding tendencies.
        /// </summary>
        private void PrintContestantsVerbose()
        {
            var contestantsChallengeSummary = new StringBuilder();
            foreach (var contestant in ContestantsDrawn)
            {
                if (contestant.ChallengedBy != null && contestant.ContestantToChallenge != null)
                {
                    contestantsChallengeSummary.AppendLine(
                        String.Format(Texts.DRAFT_TEMPLATE_PARTICIPANT_WILL_CHALLENGE,
                        contestant.Name,
                        contestant.ContestantToChallenge.Name,
                        contestant.ChallengedBy.Name));
                }
            }
            UIHelper.WriteLine(contestantsChallengeSummary.ToString(), animate: true, speed: UIHelper.WriteSpeed.Normal);
            LogResults(contestantsChallengeSummary.ToString());
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
                var contestant = new Contestant();
                UIHelper.WriteLine("");
                UIHelper.Write(String.Format("{0} {1}: ", Texts.DRAFT_CONTESTANT_NUMBER, ContestantPool.Count + 1));
                Console.ForegroundColor = contestant.Color;
                var name = Console.ReadLine();
                Console.ResetColor();
                contestant.Name = name;
                ContestantPool.Add(contestant);
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
                Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);
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
        }

        /// <summary>
        /// Draw random contestant from the contestant pool
        /// </summary>
        public void DrawContestant()
        {
            var number = new Random().Next(0, ContestantPool.Count);
            ContestantsDrawn.Add(ContestantPool.ElementAt(number));
            ContestantPool.RemoveAt(number);
            UIHelper.Write(String.Format("{0}:",Texts.DRAFT_CONTESTANT_DRAWN), animate: false);
            var contestant = ContestantsDrawn.Last();
            UIHelper.WriteLine(contestant.Name);
            UIHelper.AwaitUserInput();
        }

        /// <summary>
        /// Log all results to a date formatted log file.
        /// </summary>
        private void LogResults(string logString)
        {
            var path = "RCD-" + _initiatedDateTime.ToString("yyyy-MM-dd-hh-mm") + ".log"; // draft-2016-08-27-06-17.log
            File.AppendAllText(path, logString + Environment.NewLine);
        }

        private void LogResultsHeadline(string logString)
        {
            var numDashes = logString.Length + 8;
            var wrappingDashLine = String.Concat(Enumerable.Repeat("-", numDashes));
            var prefixsuffix = String.Concat(Enumerable.Repeat("-", 3));

            LogResults(
                wrappingDashLine + Environment.NewLine +
                String.Format("{0} {1} {2}",prefixsuffix, logString, prefixsuffix) +  Environment.NewLine +
                wrappingDashLine + Environment.NewLine + Environment.NewLine
            );
        }
    }
}
