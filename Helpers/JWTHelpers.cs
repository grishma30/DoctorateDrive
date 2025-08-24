using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DoctorateDrive.Helpers
{
    public class JWTHelpers
    {
        private readonly IConfiguration _configuration;

        public JWTHelpers(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public string GenerateToken(string userId, string email, string role)
        {
            try
            {
                var secret = _configuration["JwtSettings:Secret"];
                if (string.IsNullOrEmpty(secret))
                    throw new InvalidOperationException("JWT Secret is missing from configuration");

                if (secret.Length < 32)
                    throw new InvalidOperationException("JWT Secret must be at least 32 characters long");

                var key = Encoding.UTF8.GetBytes(secret);
                var tokenHandler = new JwtSecurityTokenHandler();

                var claims = new[]
                {
                    new Claim("userId", userId),
                    new Claim("email", email),
                    new Claim("role", role),
                    new Claim(JwtRegisteredClaimNames.Sub, userId),
                    new Claim(JwtRegisteredClaimNames.Email, email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat,
                        DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                        ClaimValueTypes.Integer64)
                };

                var expiryMinutes = 60; // Default
                if (int.TryParse(_configuration["JwtSettings:ExpiryMinutes"], out int configExpiry) && configExpiry > 0)
                {
                    expiryMinutes = configExpiry;
                }

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddMinutes(expiryMinutes),
                    Issuer = _configuration["JwtSettings:Issuer"],
                    Audience = _configuration["JwtSettings:Audience"],
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Token generation failed: {ex.Message}", ex);
            }
        }

        public ClaimsPrincipal ValidateToken(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                    throw new ArgumentException("Token cannot be null or empty");

                var tokenHandler = new JwtSecurityTokenHandler();
                var secret = _configuration["JwtSettings:Secret"];

                if (string.IsNullOrEmpty(secret))
                    throw new InvalidOperationException("JWT Secret is missing from configuration");

                var key = Encoding.UTF8.GetBytes(secret);

                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["JwtSettings:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["JwtSettings:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);
                return principal;
            }
            catch (Exception ex)
            {
                throw new SecurityTokenException($"Token validation failed: {ex.Message}", ex);
            }
        }

        public string DiagnoseToken(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                    return "Token is null or empty";

                var parts = token.Split('.');
                if (parts.Length != 3)
                    return $"Invalid token format. Expected 3 parts, got {parts.Length}";

                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadJwtToken(token);

                var diagnosis = new StringBuilder();
                diagnosis.AppendLine($"Token Type: {jsonToken.Header.Typ}");
                diagnosis.AppendLine($"Algorithm: {jsonToken.Header.Alg}");
                diagnosis.AppendLine($"Issuer: {jsonToken.Issuer}");
                diagnosis.AppendLine($"Audience: {string.Join(", ", jsonToken.Audiences)}");
                diagnosis.AppendLine($"Expires: {jsonToken.ValidTo:yyyy-MM-dd HH:mm:ss} UTC");
                diagnosis.AppendLine($"Current Time: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
                diagnosis.AppendLine($"Is Expired: {jsonToken.ValidTo < DateTime.UtcNow}");

                return diagnosis.ToString();
            }
            catch (Exception ex)
            {
                return $"Error diagnosing token: {ex.Message}";
            }
        }
    }
}
