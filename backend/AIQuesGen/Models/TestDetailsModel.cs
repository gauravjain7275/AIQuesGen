namespace AIQuesGen.Models
{
    public class TestDetailsModel
    {
        public int TestId { get; set; }
        public string TestTitle { get; set; }
        public int NumberOfQuestions { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TotalAttempts { get; set; }
        public int? TestDuration { get; set; } // NEW (optional)
        public decimal? PassingScore { get; set; } // NEW (optional)
        public DateTime? LinkExpiryDate { get; set; } // NEW (optional)
        public int? MaxAttemptsTotal { get; set; } // NEW (optional)
        public int? MaxAttemptsPerStudent { get; set; } // NEW (optional)
    }
}
