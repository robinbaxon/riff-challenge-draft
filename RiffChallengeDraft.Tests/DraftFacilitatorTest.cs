using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RiffChallengeDraft.Cli;

namespace RiffChallengeDraft.Tests
{
    [TestClass]
    public class DraftFacilitatorTest
    {
        DraftFacilitator enabler = new DraftFacilitator();

        /// <summary>
        /// If we can draw 1000 times and the number of successfull theme weeks is less than 30%, then we need to adjust the algorithm
        /// </summary>
        [TestMethod]
        public void TestThemeWeekProbability()
        {
            const int NUM_ITERATIONS = 100000;
            int iterations = NUM_ITERATIONS;
            int successCount = 0;
            while (iterations > 0)
            {
                successCount += (enabler.WeeklyTheme.IsThemeWeek) ? 1 : 0;
                iterations--;
            }
            //Assume over 30% success rate when choosing WeeklyTheme
            Assert.IsTrue(successCount > 0 && successCount  > (NUM_ITERATIONS / 3));
        }

        [TestMethod]
        public void TestConstructor()
        {
            Assert.IsTrue(enabler.ContestantPool != null);
        }
    }
}
