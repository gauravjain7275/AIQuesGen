namespace AIQuesGen.Models
{
    public class TestSubmissionRequest
    {
        public int TestId { get; set; }
        public string ParticipantName { get; set; }
        public string ParticipantEmail { get; set; }
        public Dictionary<int, string> Answers { get; set; }
        public Decimal Score { get; set; }
        public DateTime SubmittedAt { get; set; }
    }
}
