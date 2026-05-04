using AIQuesGen.Models;
using AIQuesGen.Models.Auth;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace AIQuesGen.Services
{
    public class AuthService
    {
        private readonly string _connection;

        public AuthService(IConfiguration config)
        {
            _connection = config.GetConnectionString("DefaultConnection");
        }

        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes);
        }

        public int SignUp(SignupRequest request)
        {
            using var conn = new SqlConnection(_connection);
            using var cmd = new SqlCommand("sp_SignUpUser", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@FullName", request.UserName);
            cmd.Parameters.AddWithValue("@Email", request.Email);
            cmd.Parameters.AddWithValue("@PasswordHash", HashPassword(request.Password));
            cmd.Parameters.AddWithValue("@UserType", request.UserType);

            conn.Open();
            var result = cmd.ExecuteScalar();
            return Convert.ToInt32(result);
        }

        public UserModel Login(LoginRequest request)
        {
            using var conn = new SqlConnection(_connection);
            using var cmd = new SqlCommand("sp_LoginUser", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Email", request.Email);
            cmd.Parameters.AddWithValue("@PasswordHash", HashPassword(request.Password));

            conn.Open();
            using var reader = cmd.ExecuteReader();

            if (!reader.Read()) return null;

            return new UserModel
            {
                UserId = reader.GetInt32(0),
                UserName = reader.GetString(1),
                Email = reader.GetString(2),
                UserType = reader.GetString(3)
            };
        }
    }
}
