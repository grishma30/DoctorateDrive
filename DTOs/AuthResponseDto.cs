namespace DoctorateDrive.DTOs
{
    public class AuthResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public object? Data { get; set; }
        //public string Token { get; set; } = string.Empty;
        //public string JWTtoken { get; set; } = string.Empty;

        public string? Token { get; set; }

    }
}
