using DoctorateDrive.DTOs;

namespace DoctorateDrive.Services
{
    public interface IAuthService
    {
        //Task<AuthResponseDto> RegisterUserAsync(RegisterRequestDto registerRequest);
        //Task<AuthResponseDto> LoginAsync(LoginRequestDto loginRequest);
        //Task<AuthResponseDto> ResendOtpAsync(string emailAddress);
        //Task<AuthResponseDto> ValidateTokenAsync(string token);
        //Task<AuthResponseDto> LogoutAsync(string token);

        Task<AuthResponseDto> RegisterUserAsync(RegisterRequestDto registerRequest);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto loginRequest);
        Task<AuthResponseDto> ResendOtpAsync(string emailAddress);
        Task<AuthResponseDto> ValidateTokenAsync(string token);
        Task<AuthResponseDto> LogoutAsync(string token);

    }
}
