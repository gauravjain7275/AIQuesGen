using System.Net.Http.Json;
using System.Text.Json;

namespace AIQuesGen.Services
{
    public class AIService
    {
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpClientFactory;
        private string _apiKey;

        public AIService(IConfiguration config, IHttpClientFactory httpClientFactory)
        {
            _config = config;
            _httpClientFactory = httpClientFactory;
            _apiKey = _config["OpenAI:ApiKey"];
        }

        public async Task<string> GenerateQuestionsAsync(string text, string? questionType = "mixed", int questionCount = 10, string difficulty = "Medium", string language = "Hindi")
        {
            // sanitize/normalize input
            var qType = (questionType ?? "mixed").ToLowerInvariant();
            var diff = (difficulty ?? "Medium");
            var lang = string.IsNullOrWhiteSpace(language) ? "Hindi" : language;

            // Decide breakdown for mixed: distribute types roughly
            string typeInstruction = qType switch
            {
                "mcq" => $"Create {questionCount} multiple-choice questions (MCQs) with 4 options each, and mark the correct option (A/B/C/D).",
                "short" => $"Create {questionCount} short-answer questions that check key concepts.",
                "descriptive" => $"Create {questionCount} descriptive/long-answer questions requiring detailed answers.",
                _ => $"Create {questionCount} well-balanced questions of mixed types (MCQs, short answer, descriptive). Aim for a useful mix of question forms."
            };

            // Difficulty guidance
            string diffInstruction = diff.ToLowerInvariant() switch
            {
                "easy" => "Make questions predominantly easy-level (straightforward knowledge & comprehension).",
                "hard" => "Make questions predominantly hard-level (application / analysis / higher-order thinking).",
                _ => "Make questions of moderate difficulty (mix of easy and medium)."
            };

            string instruction = $"{typeInstruction}\n" +
                                 $"{diffInstruction}\n" +
                                 $"Language: {lang}.\n\n" +
                                 $"Format STRICTLY as follows for each question:\n" +
                                 $"Q1. <question text>\n" +
                                 $"A. <option A>\nB. <option B>\nC. <option C>\nD. <option D>\nCorrect: <A|B|C|D>\n\n" +
                                 $"For non-MCQ types (short/descriptive), use:\n" +
                                 $"Q1. <question text>\nAnswer: <short answer / model answer>\n\n" +
                                 $"Return exactly {questionCount} questions. Do not add explanations or extra commentary. Use numbering Q1., Q2., ...";

            // truncate source text to a safe size (if huge)
            string truncatedText = text.Length > 6000 ? text.Substring(0, 6000) : text;

            var prompt = $"{instruction}\n\nSource:\n{truncatedText}";
            var result = await CallOpenAIAsync("You are an expert exam question generator.", prompt, 0.6, 2500);

            return result?.ToString() ?? "No questions generated.";
        }

        public async Task<string> GenerateMCQsForTest(string text, int questionCount, string difficulty = "Medium", string language = "Hindi")
        {
           string instruction = $"Generate exactly {questionCount} MCQs. " +
                                 $"Language: {language}\n" +
                                 $"Difficulty: {difficulty}\n" +
                                 $"Each question must have 4 options (A, B, C, D) " +
                                 $"and clearly mark the correct option. Format strictly as:\n\n" +
                                 $"Q1. question text\n" +
                                 $"A. option 1\nB. option 2\nC. option 3\nD. option 4\nCorrect: B\n\n" +
                                 $"NO explanations.";

            string truncated = text.Length > 4000 ? text[..4000] : text;

            string prompt = $"{instruction}\n\nContent:\n{truncated}";
            var result = await CallOpenAIAsync("You are an MCQ generation expert.", prompt, 0.6, 2500);

            return result?.ToString();
        }

        private async Task<string> CallOpenAIAsync(string systemPrompt, string userPrompt, double temperature = 0.6, int maxTokens = 2500)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Remove("Authorization");
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

            var body = new
            {
                model = "gpt-4o-mini",
                messages = new[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = userPrompt }
                },
                temperature,
                max_tokens = maxTokens
            };

            var response = await client.PostAsJsonAsync("https://api.openai.com/v1/chat/completions", body);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"OpenAI error: {error}");
            }

            var json = await response.Content.ReadFromJsonAsync<JsonDocument>();
            string result = json.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();
            return result.Trim();
        }
    }
}
