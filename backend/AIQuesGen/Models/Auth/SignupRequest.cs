namespace AIQuesGen.Models.Auth
{
    public class SignupRequest
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string UserType { get; set; }
    }
}
