namespace Ecommerce.Domain.Settings
{
    public class JwtSettings
    {
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
        public int AccessTokenExpiryMinutes { get; set; }
        public int RefreshTokenExpiryDays { get; set; }
    }
}