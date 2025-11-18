using DoctorateDrive.DTOs;
using DoctorateDrive.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoctorateDrive.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterRequestDto registerRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _authService.RegisterUserAsync(registerRequest);

                if (result.Success)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during user registration");
                return StatusCode(500, new AuthResponseDto
                {
                    Success = false,
                    Message = "Internal server error occurred during registration"
                });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto loginRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _authService.LoginAsync(loginRequest);

                if (result.Success)
                {
                    return Ok(result);
                }

                return Unauthorized(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login");
                return StatusCode(500, new AuthResponseDto
                {
                    Success = false,
                    Message = "Internal server error occurred during login"
                });
            }
        }



        [HttpPost("resend-otp")]
        public async Task<ActionResult<AuthResponseDto>> ResendOtp([FromBody] ResendOtpRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _authService.ResendOtpAsync(request.EmailAddress);

                if (result.Success)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during OTP resend");
                return StatusCode(500, new AuthResponseDto
                {
                    Success = false,
                    Message = "Internal server error occurred during OTP resend"
                });
            }
        }

        [HttpPost("validate-token")]
        [Authorize]
        public async Task<ActionResult<AuthResponseDto>> ValidateToken()
        {
            try
            {
                var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

                if (string.IsNullOrEmpty(token))
                {
                    return BadRequest(new AuthResponseDto
                    {
                        Success = false,
                        Message = "Token is required"
                    });
                }

                var result = await _authService.ValidateTokenAsync(token);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during token validation");
                return StatusCode(500, new AuthResponseDto
                {
                    Success = false,
                    Message = "Internal server error occurred during token validation"
                });
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult<AuthResponseDto>> Logout()
        {
            try
            {
                var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

                if (string.IsNullOrEmpty(token))
                {
                    return BadRequest(new AuthResponseDto
                    {
                        Success = false,
                        Message = "Token is required"
                    });
                }

                var result = await _authService.LogoutAsync(token);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during logout");
                return StatusCode(500, new AuthResponseDto
                {
                    Success = false,
                    Message = "Internal server error occurred during logout"
                });
            }
        }
    }
}
