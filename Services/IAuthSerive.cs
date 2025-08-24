using DoctorateDrive.DTOs;
using System.Threading.Tasks;

namespace DoctorateDrive.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterUserAsync(RegisterRequestDto registerRequest);
        Task<AuthResponseDto> GenerateOtpAsync(GenerateOtpRequestDto otpRequest);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto loginRequest);
        Task<AuthResponseDto> GetNewOtpAsync(string emailAddress);
        Task<AuthResponseDto> ValidateTokenAsync(string token);
        Task<AuthResponseDto> LogoutAsync(string token); // ADD THIS LINE
    }
}
