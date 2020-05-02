using NUnit.Framework;
using RiffChallengeDraft.Cli;

namespace RiffChallengeDraft.Tests
{
    public class DraftFacilitatorTest
    {
        DraftFacilitator enabler = new DraftFacilitator();

        /// <summary>
        /// If we can draw 1000 times and the number of successfull theme weeks is less than 30%, then we need to adjust the algorithm
        /// </summary>
        [Test]
        public void TestThemeWeekProbability()
        {
            // Arrage
            const int NUM_ITERATIONS = 100000;
            int iterations = NUM_ITERATIONS;
            int successCount = 0;
            
            // Act
            while (iterations > 0)
            {
                successCount += (enabler.WeeklyTheme.IsThemeWeek) ? 1 : 0;
                iterations--;
            }
            // Assert
            // Assume over 30% success rate when choosing WeeklyTheme
            Assert.Multiple(() =>
            {
                Assert.That(successCount, Is.Positive);
                Assert.That(successCount, Is.GreaterThan(NUM_ITERATIONS / 3));
            });
        }

        [Test]
        public void TestConstructor()
        {
            Assert.That(enabler.ContestantPool, Is.Not.Null);
        }
    }
}
