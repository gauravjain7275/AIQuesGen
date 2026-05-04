using AIQuesGen.Models;
using AIQuesGen.Repository;

namespace AIQuesGen.Services
{
    public class OnlineTestService
    {
        private readonly PdfService _pdfService;
        private readonly AIService _aiService;
        private readonly TestRepository _repo;

        public OnlineTestService(PdfService pdfService, AIService aiService, TestRepository repo)
        {
            _pdfService = pdfService;
            _aiService = aiService;
            _repo = repo;
        }

        public async Task<int> CreateOnlineTest(TestCreateRequest request)
        {
            string text = _pdfService.ExtractTextFromPdf(request.File);

            string aiOutput = await _aiService.GenerateMCQsForTest(text, request.QuestionCount, request.Difficulty, request.Language);

            List<QuestionModel> parsedQuestions = QuestionParser.Parse(aiOutput);

            int testId = _repo.CreateTest(request);

            foreach (var q in parsedQuestions)
                _repo.InsertQuestion(q, testId);

            return testId;
        }

        public decimal CalculateScore(Dictionary<int, string> studentAns, List<QuestionModel> actual)
        {
            int score = 0;

            foreach (var a in studentAns)
            {
                var q = actual.FirstOrDefault(x => x.QuestionId == a.Key);
                if (q != null && q.CorrectOption == a.Value)
                    score++;
            }

            return score;
        }
    }
}
