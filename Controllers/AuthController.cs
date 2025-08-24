using DoctorateDrive.DTOs;
using DoctorateDrive.Helpers;
using DoctorateDrive.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DoctorateDrive.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly JWTHelpers _jwtHelpers;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IAuthService authService,
            JWTHelpers jwtHelpers,
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _jwtHelpers = jwtHelpers;
            _logger = logger;
        }

        /// <summary>
        /// Register a new user
        /// </summary>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _logger.LogInformation("Registration attempt for email: {Email}", request.EmailId);
                var result = await _authService.RegisterUserAsync(request);

                if (result.Success)
                {
                    _logger.LogInformation("Registration successful for email: {Email}", request.EmailId);
                    return Ok(result);
                }

                _logger.LogWarning("Registration failed for email: {Email}. Reason: {Message}",
                    request.EmailId, result.Message);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Registration endpoint error for email: {Email}", request.EmailId);
                return StatusCode(500, new AuthResponseDto
                {
                    Success = false,
                    Message = "An internal server error occurred"
                });
            }
        }

        /// <summary>
        /// Generate OTP for user login
        /// </summary>
        [HttpPost("generate-otp")]
        [AllowAnonymous]
        public async Task<IActionResult> GenerateOtp([FromBody] GenerateOtpRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _logger.LogInformation("OTP generation request for email: {Email}", request.EmailId);
                var result = await _authService.GenerateOtpAsync(request);

                if (result.Success)
                {
                    _logger.LogInformation("OTP generated successfully for email: {Email}", request.EmailId);
                    return Ok(result);
                }

                _logger.LogWarning("OTP generation failed for email: {Email}. Reason: {Message}",
                    request.EmailId, result.Message);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Generate OTP endpoint error for email: {Email}", request.EmailId);
                return StatusCode(500, new AuthResponseDto
                {
                    Success = false,
                    Message = "An internal server error occurred"
                });
            }
        }

        /// <summary>
        /// Login with email and OTP, returns JWT token
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _logger.LogInformation("Login attempt for email: {Email}", request.EmailOrMobile);
                var result = await _authService.LoginAsync(request);

                if (result.Success)
                {
                    _logger.LogInformation("Login successful for email: {Email}", request.EmailOrMobile);

                    // Set JWT token in response header (optional)
                    Response.Headers.Add("Authorization", $"Bearer {result.Token}");

                    return Ok(result);
                }

                _logger.LogWarning("Login failed for email: {Email}. Reason: {Message}",
                    request.EmailOrMobile, result.Message);
                return Unauthorized(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login endpoint error for email: {Email}", request.EmailOrMobile);
                return StatusCode(500, new AuthResponseDto
                {
                    Success = false,
                    Message = "An internal server error occurred"
                });
            }
        }

        /// <summary>
        /// Generate new OTP for existing user
        /// </summary>
        [HttpPost("resend-otp")]
        [AllowAnonymous]
        public async Task<IActionResult> ResendOtp([FromBody] ResendOtpRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _logger.LogInformation("Resend OTP request for email: {Email}", request.EmailId);
                var result = await _authService.GetNewOtpAsync(request.EmailId);

                if (result.Success)
                {
                    _logger.LogInformation("OTP resent successfully for email: {Email}", request.EmailId);
                    return Ok(result);
                }

                _logger.LogWarning("Resend OTP failed for email: {Email}. Reason: {Message}",
                    request.EmailId, result.Message);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Resend OTP endpoint error for email: {Email}", request.EmailId);
                return StatusCode(500, new AuthResponseDto
                {
                    Success = false,
                    Message = "An internal server error occurred"
                });
            }
        }

        /// <summary>
        /// Validate JWT token
        /// </summary>
        [HttpPost("validate-token")]
        [AllowAnonymous]
        public async Task<IActionResult> ValidateToken([FromBody] TokenValidationDto request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Token))
                {
                    return BadRequest(new AuthResponseDto
                    {
                        Success = false,
                        Message = "Token is required"
                    });
                }

                _logger.LogDebug("Token validation request received");
                var result = await _authService.ValidateTokenAsync(request.Token);

                if (result.Success)
                {
                    _logger.LogDebug("Token validation successful");
                    return Ok(result);
                }

                _logger.LogWarning("Token validation failed: {Message}", result.Message);
                return Unauthorized(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Token validation endpoint error");
                return StatusCode(500, new AuthResponseDto
                {
                    Success = false,
                    Message = "An internal server error occurred"
                });
            }
        }

        /// <summary>
        /// Logout user (invalidate token)
        /// </summary>
        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var authHeader = Request.Headers["Authorization"].ToString();
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    return BadRequest(new AuthResponseDto
                    {
                        Success = false,
                        Message = "Invalid authorization header"
                    });
                }

                var token = authHeader.Replace("Bearer ", "");
                _logger.LogInformation("Logout request received");

                var result = await _authService.LogoutAsync(token);

                if (result.Success)
                {
                    _logger.LogInformation("Logout successful");
                    return Ok(result);
                }

                _logger.LogWarning("Logout failed: {Message}", result.Message);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logout endpoint error");
                return StatusCode(500, new AuthResponseDto
                {
                    Success = false,
                    Message = "An internal server error occurred"
                });
            }
        }

        /// <summary>
        /// Get current user information (protected endpoint)
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var authHeader = Request.Headers["Authorization"].ToString();
                var token = authHeader.Replace("Bearer ", "");

                _logger.LogDebug("Get current user request");
                var result = await _authService.ValidateTokenAsync(token);

                if (result.Success)
                {
                    _logger.LogDebug("Current user retrieved successfully");
                    return Ok(new
                    {
                        success = true,
                        message = "User information retrieved successfully",
                        data = result.Data,
                        timestamp = DateTime.UtcNow
                    });
                }

                _logger.LogWarning("Get current user failed: {Message}", result.Message);
                return Unauthorized(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Get current user endpoint error");
                return StatusCode(500, new
                {
                    success = false,
                    message = "An internal server error occurred"
                });
            }
        }

        /// <summary>
        /// Protected test endpoint to verify JWT authentication
        /// </summary>
        [HttpGet("protected")]
        [Authorize]
        public IActionResult GetProtectedData()
        {
            try
            {
                var userId = User.FindFirst("userId")?.Value;
                var email = User.FindFirst("email")?.Value;
                var role = User.FindFirst("role")?.Value;

                _logger.LogDebug("Protected endpoint accessed by user: {UserId}", userId);

                return Ok(new
                {
                    success = true,
                    message = "This is protected data accessible only with valid JWT token",
                    data = new
                    {
                        userId = userId,
                        email = email,
                        role = role,
                        timestamp = DateTime.UtcNow,
                        serverInfo = "DoctorateDrive API v1.0"
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Protected endpoint error");
                return StatusCode(500, new
                {
                    success = false,
                    message = "An internal server error occurred"
                });
            }
        }

        /// <summary>
        /// Test database connectivity
        /// </summary>
        [HttpGet("test-database")]
        [AllowAnonymous]
        public async Task<IActionResult> TestDatabase()
        {
            try
            {
                _logger.LogInformation("Database connectivity test requested");

                using var scope = HttpContext.RequestServices.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<DoctorateDrive.Data.DoctorateDriveContext>();

                var canConnect = await context.Database.CanConnectAsync();

                if (!canConnect)
                {
                    _logger.LogError("Database connection test failed");
                    return StatusCode(500, new
                    {
                        success = false,
                        message = "Cannot connect to database"
                    });
                }

                // Test OTP table access
                var otpCount = await context.OtpVerifications.CountAsync();
                var userCount = await context.Users.CountAsync();

                _logger.LogInformation("Database connectivity test successful");

                return Ok(new
                {
                    success = true,
                    message = "Database connection successful",
                    data = new
                    {
                        canConnect = true,
                        userCount = userCount,
                        otpCount = otpCount,
                        timestamp = DateTime.UtcNow
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Database connectivity test error");
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Database test failed: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Test email service
        /// </summary>
        [HttpPost("test-email")]
        [AllowAnonymous]
        public async Task<IActionResult> TestEmail([FromBody] TestEmailDto request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Email))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Email address is required"
                    });
                }

                _logger.LogInformation("Email test requested for: {Email}", request.Email);

                using var scope = HttpContext.RequestServices.CreateScope();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                var subject = "Test Email - DoctorateDrive";
                var body = $"This is a test email sent at {DateTime.Now:yyyy-MM-dd HH:mm:ss}. If you receive this, your SMTP configuration is working correctly!";

                await emailService.SendEmailAsync(request.Email, subject, body);

                _logger.LogInformation("Test email sent successfully to: {Email}", request.Email);

                return Ok(new
                {
                    success = true,
                    message = $"Test email sent successfully to {request.Email}",
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Email test failed for: {Email}", request.Email);
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Email test failed: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Diagnose JWT token (for debugging)
        /// </summary>
        [HttpPost("diagnose-token")]
        [AllowAnonymous]
        public IActionResult DiagnoseToken([FromBody] TokenValidationDto request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Token))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Token is required"
                    });
                }

                _logger.LogDebug("Token diagnosis requested");
                var diagnosis = _jwtHelpers.DiagnoseToken(request.Token);

                return Ok(new
                {
                    success = true,
                    message = "Token diagnosis completed",
                    diagnosis = diagnosis,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Token diagnosis error");
                return StatusCode(500, new
                {
                    success = false,
                    message = $"Token diagnosis failed: {ex.Message}"
                });
            }
        }
    }
}
