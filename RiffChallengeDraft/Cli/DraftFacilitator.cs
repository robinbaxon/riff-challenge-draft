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
        private string _logFilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private string _fileName;
        private string _absolutePath;
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
            _fileName = "RCD-" + _initiatedDateTime.ToString("yyyy-MM-dd-hh-mm") + ".log"; // draft-2016-08-27-06-17.log
            _absolutePath = _logFilePath + "\\" + _fileName;
        }

     

        public void StartDraft()
        {
            LogResultsHeadline("Riff Challenge - " + _initiatedDateTime.ToLongDateString());
            WriteWelcomeText();
            ReadContestantsFromConsole();
            DrawAllContestants();
            AddContestantChallenges();
            PrintContestantsChallengeOrder();
            
            UIHelper.WriteLine(Texts.ORDER_DETERMINED_ON_TO_THEME);
            UIHelper.AwaitUserInput();
            PerformThemeWeekDraft();
            PerformStreamerSecretaryDraft();
            

            UIHelper.AwaitUserInput();
            UIHelper.WriteLine(String.Format("{0}: {1}", Texts.LOG_CAN_BE_FOUND_HERE, (_absolutePath)));
            UIHelper.WriteLine(Texts.ENDING_TEXT);
            UIHelper.AwaitUserInput();
        }


        /// <summary>
        /// Print each contestant, who they challenge and who the are challenged by. 
        /// </summary>
        private void PrintContestantsChallengeOrder()
        {
            LogResultsHeadline(Texts.DRAFT_CHALLENGE_OVERVIEW);

            if (PrintVerboseChallengeOrder)
            {
                PrintContestantsVerbose();
            }

            /// String table, for nice log outputs
            var contestantsChallengeTable = ContestantsDrawn.ToStringTable(
                cont => cont.Name,
                cont => cont.ChallengedBy,
                cont => cont.ContestantToChallenge,
                cont => cont.Color
                );
            LogResults(contestantsChallengeTable);
            UIHelper.WriteLine(contestantsChallengeTable, animate: true, speed: UIHelper.WriteSpeed.Fast);
            UIHelper.AwaitUserInput();
        }

        /// <summary>
        /// Theme week draft for next week's riff challenge.
        /// </summary>
        private void PerformThemeWeekDraft()
        {
            LogResultsHeadline(Texts.THEME_WEEK);
            UIHelper.WriteLine(Texts.THEME_WEEK);
            // Is it wildcard week?
            UIHelper.WriteLine(Texts.THEME_QUESTION);
            UIHelper.AwaitUserInput();
            var themeweekString = WeeklyTheme.IsThemeWeek ? Texts.THEME_YES : Texts.THEME_NO;
            UIHelper.WriteLine(
                themeweekString, 
                speed: UIHelper.WriteSpeed.ExtraSlow, 
                color: WeeklyTheme.IsThemeWeek ? ConsoleColor.Green : ConsoleColor.Red
            );
            LogResults("Wildcard: " + themeweekString);
            if (WeeklyTheme.IsThemeWeek)
            {
                UIHelper.WriteLine(Texts.THEME_CHOICES_ARE + ": " + WeeklyTheme.WildcardChoices);
                UIHelper.AwaitUserInput();
                _weeklyWildcardGenre = WeeklyTheme.GetRandomGenre();
                LogResults("Theme type: " + _weeklyWildcardGenre.ToString());
                UIHelper.WriteLine(Texts.THEME_PICK_IS + _weeklyWildcardGenre.ToString());
                UIHelper.AwaitUserInput();
                UIHelper.WriteLine(String.Format(Texts.THEME_TEMPLATE_WHICH_CONTESTANT, _weeklyWildcardGenre));
                var _weeklyWildcardContestant = WeeklyTheme.GetRandomContestant(ContestantsDrawn);
                UIHelper.AwaitUserInput();
                UIHelper.WriteLine(String.Format(Texts.THEME_TEMPLATE_PARTICIPANT_PICK, _weeklyWildcardContestant.Name), animate: true);
                LogResults("Pick theme from: " + _weeklyWildcardContestant.Name);
            }
        }

        /// <summary>
        /// Streamer draft for next week's riff challenge.
        /// </summary>
        private void PerformStreamerSecretaryDraft()
        {
            LogResultsHeadline(Texts.NEXTDRAFT_HEADLINE);
            UIHelper.WriteLine(Texts.NEXTDRAFT_HEADLINE);
            var streamer = WeeklyTheme.GetRandomContestant(ContestantsDrawn);
            
            var streamerTextLine = String.Format("{0}: {1}", Texts.NEXTDRAFT_STREAMER_IS, streamer.Name);
            UIHelper.AwaitUserInput();
            UIHelper.WriteLine(streamerTextLine);
            UIHelper.AwaitUserInput();

            var secretaryList = ContestantsDrawn;
            if (secretaryList.Count > 1) // In case there is only ONE contestant! 
            {
                secretaryList.Remove(streamer); 
            }
            var secretary = WeeklyTheme.GetRandomContestant(secretaryList);
            var secretaryTextLine = String.Format("{0}: {1}", Texts.NEXTDRAFT_SECRETARY_IS, secretary.Name);
            UIHelper.WriteLine(secretaryTextLine);
            LogResults(streamerTextLine);
            LogResults(secretaryTextLine);
        }


        private void WriteWelcomeText()
        {
            UIHelper.WriteLine(Texts.WELCOME_TEXT, color: ConsoleColor.Red);
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
            LogResultsHeadline(Texts.DRAFT_INITIAL_PARTICIPANTS);
            LogResults(ContestantPool.ToStringTable(
                cont => cont.Name
                ));
        }
        /// <summary>
        /// Randomize all contestants to get the draft order.
        /// </summary>
        public void DrawAllContestants()
        {
            UIHelper.WriteLine(Texts.DRAFT_CONTINUE, true);
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
            UIHelper.WriteLine(contestant.Name, color: contestant.Color);
            UIHelper.AwaitUserInput();
        }

        /// <summary>
        /// Log all results to a date formatted log file.
        /// </summary>
        private void LogResults(string logString)
        {
            //File.AppendAllText(_absolutePath, logString + Environment.NewLine);
            using (FileStream fs = new FileStream(_absolutePath,FileMode.Append, FileAccess.Write))
            {
                StreamWriter sw = new StreamWriter(fs);
                long endPoint=fs.Length;
                // Set the stream position to the end of the file.        
                fs.Seek(endPoint, SeekOrigin.Begin);
                sw.WriteLine(logString);
                sw.Flush();
            }
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
