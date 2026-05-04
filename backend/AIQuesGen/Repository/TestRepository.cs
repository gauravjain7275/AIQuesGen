using System.Data;
using System.Data.SqlClient;
using AIQuesGen.Models;

namespace AIQuesGen.Repository
{
    public class TestRepository
    {
        private readonly string _connectionString;

        public TestRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        public int CreateTest(TestCreateRequest request)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_CreateTest", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@CreatedByUserId", request.CreatedByUserId);
            cmd.Parameters.AddWithValue("@InstitutionId", (object)request.InstitutionId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@TestTitle", request.TestTitle);
            cmd.Parameters.AddWithValue("@NumberOfQuestions", request.QuestionCount);
            cmd.Parameters.AddWithValue("@ExpirationDate", request.LinkExpiryDate);
            cmd.Parameters.AddWithValue("@TestDuration", request.TestDuration);
            cmd.Parameters.AddWithValue("@MaxAttemptsTotal", request.MaxAttemptsTotal);
            cmd.Parameters.AddWithValue("@MaxAttemptsPerStudent", request.MaxAttemptsPerStudent);

            var output = new SqlParameter("@TestId", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(output);

            conn.Open();
            cmd.ExecuteNonQuery();

            return (int)output.Value;
        }

        public void InsertQuestion(QuestionModel q, int testId)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_InsertQuestion", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@TestId", testId);
            cmd.Parameters.AddWithValue("@QuestionText", q.QuestionText);
            cmd.Parameters.AddWithValue("@OptionA", q.OptionA);
            cmd.Parameters.AddWithValue("@OptionB", q.OptionB);
            cmd.Parameters.AddWithValue("@OptionC", q.OptionC);
            cmd.Parameters.AddWithValue("@OptionD", q.OptionD);
            cmd.Parameters.AddWithValue("@CorrectOption", q.CorrectOption);

            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public void SaveSubmission(int testId, string name, string email, decimal score)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_SaveTestSubmission", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@TestId", testId);
            cmd.Parameters.AddWithValue("@ParticipantName", name);
            cmd.Parameters.AddWithValue("@ParticipantEmail", email);
            cmd.Parameters.AddWithValue("@Score", score);

            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public List<QuestionModel> GetQuestionsByTestId(int testId)
        {
            var questions = new List<QuestionModel>();

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT * FROM Questions WHERE TestId = @id", conn);
            cmd.Parameters.AddWithValue("@id", testId);

            conn.Open();
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                questions.Add(new QuestionModel
                {
                    QuestionId = reader.GetInt32(reader.GetOrdinal("QuestionId")),
                    QuestionText = reader["QuestionText"].ToString(),
                    OptionA = reader["OptionA"].ToString(),
                    OptionB = reader["OptionB"].ToString(),
                    OptionC = reader["OptionC"].ToString(),
                    OptionD = reader["OptionD"].ToString(),
                    CorrectOption = reader["CorrectOption"].ToString()
                });
            }

            return questions;
        }

        public TestWithQuestionsDto GetTestWithQuestions(int testId)
        {
            var result = new TestWithQuestionsDto();

            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            // --- 1. Fetch Test Title ---
            using (var cmd1 = new SqlCommand("SELECT TestTitle, TestDuration FROM Tests WHERE TestId = @id", conn))
            {
                cmd1.Parameters.AddWithValue("@id", testId);

                using (var reader1 = cmd1.ExecuteReader())   // IMPORTANT
                {
                    if (reader1.Read())
                    {
                        result.Title = reader1["TestTitle"].ToString();
                        result.Duration = reader1.IsDBNull(reader1.GetOrdinal("TestDuration"))
                                          ? 0  // OR null if nullable
                                          : reader1.GetInt32(reader1.GetOrdinal("TestDuration"));
                    }
                }  // <-- reader1 CLOSED here
            }

            // --- 2. Fetch Questions ---
            using (var cmd2 = new SqlCommand("SELECT * FROM Questions WHERE TestId = @id", conn))
            {
                cmd2.Parameters.AddWithValue("@id", testId);

                using (var reader2 = cmd2.ExecuteReader())  // IMPORTANT
                {
                    result.Questions = new List<QuestionModel>();

                    while (reader2.Read())
                    {
                        result.Questions.Add(new QuestionModel
                        {
                            QuestionId = reader2.GetInt32(reader2.GetOrdinal("QuestionId")),
                            QuestionText = reader2["QuestionText"].ToString(),
                            OptionA = reader2["OptionA"].ToString(),
                            OptionB = reader2["OptionB"].ToString(),
                            OptionC = reader2["OptionC"].ToString(),
                            OptionD = reader2["OptionD"].ToString(),
                            CorrectOption = reader2["CorrectOption"].ToString()
                        });
                    }
                } // <-- reader2 CLOSED here
            }

            return result;
        }

        public TestDetailsModel GetTestDetails(int testId)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_GetTestDetailsById", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@testId", testId);

            conn.Open();
            var r = cmd.ExecuteReader();

            if (!r.Read()) return null;

            return new TestDetailsModel
            {
                TestId = testId,
                TestTitle = r["TestTitle"].ToString(),
                NumberOfQuestions = (int)r["NumberOfQuestions"],
                CreatedAt = (DateTime)r["CreatedAt"],
                TotalAttempts = (int)r["TotalAttempts"],
                LinkExpiryDate = !r.IsDBNull(r.GetOrdinal("LinkExpiryDate"))
        ? r.GetDateTime(r.GetOrdinal("LinkExpiryDate"))
        : (DateTime?)null,

                MaxAttemptsPerStudent = !r.IsDBNull(r.GetOrdinal("MaxAttemptsPerStudent"))
        ? r.GetInt32(r.GetOrdinal("MaxAttemptsPerStudent"))
        : (int?)null,

                MaxAttemptsTotal = !r.IsDBNull(r.GetOrdinal("MaxAttemptsTotal"))
        ? r.GetInt32(r.GetOrdinal("MaxAttemptsTotal"))
        : (int?)null,

                TestDuration = !r.IsDBNull(r.GetOrdinal("TestDuration"))
        ? r.GetInt32(r.GetOrdinal("TestDuration"))
        : (int?)null
            };
        }

        public List<TestDetailsModel> GetTestsByCreator(int userId)
        {
            var list = new List<TestDetailsModel>();

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_GetTestsByUserId", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@CreatedByUserId", userId);

            conn.Open();
            var r = cmd.ExecuteReader();
            while (r.Read())
            {
                list.Add(new TestDetailsModel
                {
                    TestId = r.GetInt32(0),
                    TestTitle = r.GetString(1),
                    CreatedAt = r.GetDateTime(2),
                    NumberOfQuestions = r.GetInt32(3)
                });
            }

            return list;
        }

        public List<TestSubmissionRequest> GetTestAttempts(int testId)
        {
            var result = new List<TestSubmissionRequest>();

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_GetTestAttemptsById", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@testId", testId);

            conn.Open();
            var r = cmd.ExecuteReader();

            while (r.Read())
            {
                result.Add(new TestSubmissionRequest
                {
                    ParticipantName = r["ParticipantName"].ToString(),
                    ParticipantEmail = r["ParticipantEmail"].ToString(),
                    Score = (Decimal)r["Score"],
                    SubmittedAt = (DateTime)r["SubmittedAt"]
                });
            }
            return result;
        }

        public int GetAttemptsCountByStudent(int testId, string studentEmailOrId)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("sp_GetAttemptsByStudent", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@id", testId);
            cmd.Parameters.AddWithValue("@email", (object)studentEmailOrId ?? DBNull.Value);

            conn.Open();
            return (int)cmd.ExecuteScalar();
        }
    }
}
