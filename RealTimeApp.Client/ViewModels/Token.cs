namespace RealTimeApp.Client.ViewModels
{
    public class TokenResponse
    {
        public Token Token { get; set; }
    }

    public class Token
    {
        public string AccessToken { get; set; }
        public DateTime Expiration { get; set; }
        public string RefreshToken { get; set; }
    }
}
