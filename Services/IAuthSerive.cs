using DoctorateDrive.DTOs;
using System.Threading.Tasks;

namespace DoctorateDrive.Services
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterUserAsync(RegisterRequestDto registerRequest);
        Task<AuthResponseDto> GenerateOtpAsync(GenerateOtpRequestDto otpRequest);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto loginRequest);
        Task<AuthResponseDto> GetNewOtpAsync(string emailOrMobile);
    }
}
