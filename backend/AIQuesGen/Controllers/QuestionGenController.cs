using AIQuesGen.Models;
using AIQuesGen.Services;
using Microsoft.AspNetCore.Mvc;

namespace AIQuesGen.Controllers
{
    [ApiController]
    [Route("question")]
    public class QuestionGenController : ControllerBase
    {
        private readonly PdfService _pdfService;
        private readonly AIService _aiService;

        public QuestionGenController(PdfService pdfService, AIService aiService)
        {
            _pdfService = pdfService;
            _aiService = aiService;
        }

        [HttpPost("generate-questions")]
        public async Task<IActionResult> GenerateQuestions([FromForm] QuestionRequest request)
        {
            if (request.File == null || request.File.Length == 0)
                return BadRequest("Please upload a valid PDF file.");

            if (request.QuestionCount < 1) request.QuestionCount = 1;
            if (request.QuestionCount > 100) request.QuestionCount = 100;


            try
            {
                var text = _pdfService.ExtractTextFromPdf(request.File);
                if (string.IsNullOrWhiteSpace(text))
                    return BadRequest(new { error = "No text extracted from PDF." });

                var questions = await _aiService.GenerateQuestionsAsync(text, request.QuestionType ?? "mixed", request.QuestionCount, request.Difficulty ?? "Medium", request.Language ?? "Hindi");
                return Ok(new { questions });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetStatus()
        {
            return Ok(new { message = "PDF Question Generator API running successfully!" });
        }
    }
}
