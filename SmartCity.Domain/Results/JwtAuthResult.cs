namespace SmartCity.Domain.Results
{
    public class JwtAuthResult
    {
        public string AccessToken { get; set; }
        public RefreshToken refreshToken { get; set; }
    }
    public class RefreshToken
    {
        public string TokenString { get; set; }
        public DateTime ExpireAt { get; set; }
    }
}
