using RiffChallengeDraft.Cli;

namespace RiffChallengeDraft
{
    class Program
    {
        static void Main(string[] args)
        {
            var draftenabler = new DraftFacilitator();
            draftenabler.StartDraft();
        }
    }
}
