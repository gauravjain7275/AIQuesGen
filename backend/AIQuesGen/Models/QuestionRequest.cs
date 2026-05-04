namespace AIQuesGen.Models
{
    public class QuestionRequest
    {
        public IFormFile File { get; set; }

        // "mcq", "short", "descriptive", "mixed"
        public string? QuestionType { get; set; }

        // number of questions requested
        public int QuestionCount { get; set; } = 10;

        // "Easy", "Medium", "Hard" (string to keep it simple)
        public string? Difficulty { get; set; }

        // optional: language (defaults to Hindi per your SRS)
        public string? Language { get; set; } = "Hindi";
    }
}
