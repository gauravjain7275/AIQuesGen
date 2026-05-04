using AIQuesGen.Models;
using System.Text.RegularExpressions;

namespace AIQuesGen.Services
{
    public class QuestionParser
    {
        public static List<QuestionModel> Parse(string text)
        {
            var questions = new List<QuestionModel>();

            string[] blocks = Regex.Split(text, @"Q\d+\.");

            foreach (var block in blocks.Skip(1))
            {
                try
                {
                    var q = new QuestionModel();

                    var lines = block.Trim().Split("\n");

                    q.QuestionText = lines[0].Trim();

                    q.OptionA = lines.FirstOrDefault(x => x.StartsWith("A."))?.Substring(2).Trim();
                    q.OptionB = lines.FirstOrDefault(x => x.StartsWith("B."))?.Substring(2).Trim();
                    q.OptionC = lines.FirstOrDefault(x => x.StartsWith("C."))?.Substring(2).Trim();
                    q.OptionD = lines.FirstOrDefault(x => x.StartsWith("D."))?.Substring(2).Trim();

                    var correct = lines.FirstOrDefault(x => x.StartsWith("Correct"));
                    if (correct != null)
                    {
                        q.CorrectOption = correct.Split(":")[1].Trim();
                    }

                    questions.Add(q);
                }
                catch { }
            }

            return questions;
        }
    }
}
