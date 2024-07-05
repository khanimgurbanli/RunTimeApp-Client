namespace RealTimeApp.Client.ViewModels
{
    public class AppUser_ViewModel
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string? ProfilePicturePath { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; } = DateTime.UtcNow;
    }
}
