namespace AIQuesGen.Models
{
    public class TestCreateRequest
    {
        public IFormFile File { get; set; }
        public string TestTitle { get; set; }
        public int CreatedByUserId { get; set; }
        public int? InstitutionId { get; set; }
        public int QuestionCount { get; set; } = 20;
        public string Language { get; set; }      // NEW
        public string Difficulty { get; set; }    // NEW

        public int? TestDuration { get; set; } // NEW (optional)
        public decimal? PassingScore { get; set; } // NEW (optional)
        public DateTime? LinkExpiryDate { get; set; } // NEW (optional)
        public int? MaxAttemptsTotal { get; set; } // NEW (optional)
        public int? MaxAttemptsPerStudent { get; set; } // NEW (optional)
    }
}
