using AIQuesGen.Models.Auth;
using AIQuesGen.Services;
using Microsoft.AspNetCore.Mvc;

namespace AIQuesGen.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly JwtService _jwtService;

        public AuthController(AuthService authService, JwtService jwtService)
        {
            _authService = authService;
            _jwtService = jwtService;
        }

        [HttpPost("signup")]
        public IActionResult Signup(SignupRequest req)
        {
            var result = _authService.SignUp(req);

            if (result == -1)
                return BadRequest(new { message = "Email already registered" });

            return Ok(new { message = "Signup successful", userId = result });
        }

        [HttpPost("login")]
        public IActionResult Login(LoginRequest req)
        {
            var user = _authService.Login(req);

            if (user == null)
                return Unauthorized(new { message = "Invalid login credentials" });

            var token = _jwtService.GenerateToken(user);

            return Ok(new
            {
                token,
                user = new
                {
                    user.UserId,
                    user.UserName,
                    user.Email,
                    user.UserType
                }
            });
        }
    }
}
