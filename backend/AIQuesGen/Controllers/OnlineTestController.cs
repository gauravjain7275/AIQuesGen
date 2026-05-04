using AIQuesGen.Models;
using AIQuesGen.Repository;
using AIQuesGen.Services;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace AIQuesGen.Controllers
{
    [ApiController]
    [Route("test")]
    public class OnlineTestController : ControllerBase
    {
        private readonly OnlineTestService _testService;
        private readonly TestRepository _repo;

        public OnlineTestController(OnlineTestService testService, TestRepository repo)
        {
            _testService = testService;
            _repo = repo;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateTest([FromForm] TestCreateRequest request)
        {
            int testId = await _testService.CreateOnlineTest(request);
            string link = $"https://yourdomain.com/test/start/{testId}";

            return Ok(new { testId, link });
        }

        [HttpPost("submit")]
        public IActionResult SubmitTest([FromBody] TestSubmissionRequest request)
        {
            try
            {
                // 1. Fetch actual questions from DB
                var actualQuestions = _repo.GetQuestionsByTestId(request.TestId);

                // 2. Calculate score
                int total = actualQuestions.Count;
                int correct = 0;

                foreach (var q in actualQuestions)
                {
                    if (request.Answers.TryGetValue(q.QuestionId, out string submittedOption))
                    {
                        if (submittedOption == q.CorrectOption)
                            correct++;
                    }
                }

                decimal score = ((decimal)correct / total) * 100;

                // 3. Save submission
                _repo.SaveSubmission(request.TestId, request.ParticipantName, request.ParticipantEmail, score);

                return Ok(new
                {
                    success = true,
                    message = "Test submitted successfully.",
                    score = score,
                    correct = correct,
                    total = total
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("questions/{testId}")]
        public IActionResult GetTestQuestions(int testId)
        {
            var questions = _repo.GetQuestionsByTestId(testId);
            return Ok(questions);
        }

        [HttpGet("test-with-questions/{testId}")]
        public IActionResult GetTestWithQuestions(int testId)
        {
            var data = _repo.GetTestWithQuestions(testId);
            return Ok(data);
        }

        [HttpGet("{testId}")]
        public IActionResult GetTestDetails(int testId)
        {
            var details = _repo.GetTestDetails(testId);
            return Ok(details);
        }

        [HttpGet("my-tests/{userId}")]
        public IActionResult GetMyTests(int userId)
        {
            var tests = _repo.GetTestsByCreator(userId);
            return Ok(tests);
        }

        [HttpGet("attempts/{testId}")]
        public IActionResult GetTestAttempts(int testId)
        {
            var list = _repo.GetTestAttempts(testId);
            return Ok(list);
        }

        [HttpGet("validate/{testId}")]
        public IActionResult ValidateTestAccess(int testId, [FromQuery] string participantEmail = null)
        {
            try
            {
                var test = _repo.GetTestDetails(testId);
                if (test == null) return NotFound(new { allowed = false, reason = "Test not found" });

                // 1. Check link expiry (if set)
                if (test.LinkExpiryDate.HasValue && DateTime.UtcNow > test.LinkExpiryDate.Value.ToUniversalTime())
                    return Ok(new { allowed = false, reason = "Link expired" });

                // 2. Check MaxAttemptsTotal (if set)
                if (test.MaxAttemptsTotal.HasValue)
                {
                    var totalAttempts = _repo.GetTestAttempts(testId).Count();
                    if (totalAttempts >= test.MaxAttemptsTotal.Value)
                        return Ok(new { allowed = false, reason = "Test attempt limit reached" });
                }

                // 4. Check per-student limits (if provided)
                if (!string.IsNullOrWhiteSpace(participantEmail))
                {
                    // If DisableMultipleAttempts is enabled, treat as MaxAttemptsPerStudent = 1
                    if (test.MaxAttemptsPerStudent.HasValue)
                    {
                        var attemptsForStudent = _repo.GetAttemptsCountByStudent(testId, participantEmail);
                        if (attemptsForStudent >= test.MaxAttemptsPerStudent.Value)
                            return Ok(new { allowed = false, reason = "Your allowed attempts for this test are exhausted" });
                    }
                }

                // All checks passed
                return Ok(new { allowed = true });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { allowed = false, reason = ex.Message });
            }
        }

    }
}
