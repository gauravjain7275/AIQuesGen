namespace AIQuesGen.Models
{
    public class QuestionModel
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }

        public string OptionA { get; set; }
        public string OptionB { get; set; }
        public string OptionC { get; set; }
        public string OptionD { get; set; }

        public string CorrectOption { get; set; } // "A", "B", "C", or "D"
    }
}
