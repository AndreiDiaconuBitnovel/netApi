namespace WebApplication2.Models.LogIn
{
    public class AuthResult
    {
        public string Token { get; set; } = string.Empty;
        public bool IsSuccessful { get; set; }
        public List<string> Errors { get; set; }
    }
}
