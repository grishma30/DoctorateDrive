using DoctorateDrive.Data;
using DoctorateDrive.DTOs;
using DoctorateDrive.Models;
using DoctorateDrive.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace DoctorateDrive.Services
{
    public class AuthService : IAuthService
    {
        private readonly DoctorateDriveContext _context;
        private readonly IEmailService _emailService;
        private readonly JWTHelpers _jwtHelpers;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            DoctorateDriveContext context,
            IEmailService emailService,
            JWTHelpers jwtHelpers,
            ILogger<AuthService> logger)
        {
            _context = context;
            _emailService = emailService;
            _jwtHelpers = jwtHelpers;
            _logger = logger;
        }

        public async Task<AuthResponseDto> RegisterUserAsync(RegisterRequestDto registerRequest)
        {
            try
            {
                _logger.LogInformation("Starting user registration process for email: {Email}", registerRequest.EmailId);

                // Check if user already exists (email-only check)
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.EmailId == registerRequest.EmailId);

                if (existingUser != null)
                {
                    _logger.LogWarning("Registration failed: User already exists with email {Email}", registerRequest.EmailId);
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "User already exists with this email address"
                    };
                }

                var newUser = new User
                {
                    FullName = registerRequest.FullName,
                    EmailId = registerRequest.EmailId,
                    MobileNumber = registerRequest.MobileNumber,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User registered successfully with ID: {UserId}", newUser.UserId);

                // Send welcome email (non-blocking)
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await SendRegistrationEmailAsync(newUser);
                        _logger.LogInformation("Registration email sent successfully to {Email}", newUser.EmailId);
                    }
                    catch (Exception emailEx)
                    {
                        _logger.LogError(emailEx, "Registration email failed for user {UserId}", newUser.UserId);
                    }
                });

                return new AuthResponseDto
                {
                    Success = true,
                    Message = "User registered successfully. Please check your email for confirmation.",
                    Data = new { UserId = newUser.UserId }
                };
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error during registration for email: {Email}", registerRequest.EmailId);
                var sqlException = ex.InnerException as SqlException;
                var errorMessage = sqlException?.Message ?? ex.InnerException?.Message ?? ex.Message;

                return new AuthResponseDto
                {
                    Success = false,
                    Message = $"Registration failed: {errorMessage}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "General error during registration for email: {Email}", registerRequest.EmailId);
                return new AuthResponseDto
                {
                    Success = false,
                    Message = $"Registration failed: {ex.Message}"
                };
            }
        }

        public async Task<AuthResponseDto> GenerateOtpAsync(GenerateOtpRequestDto otpRequest)
        {
            try
            {
                _logger.LogInformation("Starting OTP generation for email: {Email}", otpRequest.EmailId);

                // Find user by email only
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.EmailId == otpRequest.EmailId);

                if (user == null)
                {
                    _logger.LogWarning("OTP generation failed: User not found with email {Email}", otpRequest.EmailId);
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "User not found with this email address"
                    };
                }

                _logger.LogDebug("User found for OTP generation: {UserId}", user.UserId);

                // Check rate limiting (prevent OTP spam)
                var recentOtp = await _context.OtpVerifications
                    .Where(o => o.UserId == user.UserId)
                    .OrderByDescending(o => o.CreatedAt)
                    .FirstOrDefaultAsync();

                if (recentOtp != null && recentOtp.CreatedAt.AddMinutes(1) > DateTime.Now)
                {
                    var remainingSeconds = (int)(60 - (DateTime.Now - recentOtp.CreatedAt).TotalSeconds);
                    _logger.LogWarning("OTP generation rate limited for user {UserId}. {Seconds} seconds remaining",
                        user.UserId, remainingSeconds);

                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = $"Please wait {remainingSeconds} seconds before requesting a new OTP"
                    };
                }

                // Generate secure 6-digit OTP
                var otpCode = GenerateSecureOtp();
                _logger.LogDebug("OTP generated for user {UserId}", user.UserId);

                // Remove existing OTPs for this user
                var existingOtps = _context.OtpVerifications
                    .Where(o => o.UserId == user.UserId);

                var existingCount = existingOtps.Count();
                _logger.LogDebug("Removing {Count} existing OTPs for user {UserId}", existingCount, user.UserId);

                _context.OtpVerifications.RemoveRange(existingOtps);

                // Create new OTP record
                var otpVerification = new OtpVerification
                {
                    UserId = user.UserId,
                    OtpCode = otpCode,
                    ExpiryTime = DateTime.Now.AddMinutes(10),
                    CreatedAt = DateTime.Now
                };

                _context.OtpVerifications.Add(otpVerification);

                // Save to database
                var saveResult = await _context.SaveChangesAsync();
                _logger.LogInformation("OTP record saved successfully for user {UserId}. {Changes} changes made",
                    user.UserId, saveResult);

                // Verify the record was actually saved
                var verifyRecord = await _context.OtpVerifications
                    .Where(o => o.UserId == user.UserId && o.OtpCode == otpCode)
                    .FirstOrDefaultAsync();

                if (verifyRecord == null)
                {
                    _logger.LogError("CRITICAL: OTP record not found after save for user {UserId}", user.UserId);
                    throw new Exception("Failed to save OTP record to database");
                }

                _logger.LogDebug("OTP record verified in database: ID {OtpId}", verifyRecord.OtpId);

                // Send OTP via email
                try
                {
                    await SendOtpEmailAsync(user, otpCode);
                    _logger.LogInformation("OTP email sent successfully to user {UserId}", user.UserId);
                }
                catch (Exception emailEx)
                {
                    _logger.LogError(emailEx, "OTP email sending failed for user {UserId}", user.UserId);

                    // Remove the OTP if email fails
                    _context.OtpVerifications.Remove(verifyRecord);
                    await _context.SaveChangesAsync();

                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Failed to send OTP email. Please check your email address and try again."
                    };
                }

                return new AuthResponseDto
                {
                    Success = true,
                    Message = "OTP sent successfully to your email address",
                    Data = new
                    {
                        ExpiryTime = otpVerification.ExpiryTime,
                        ExpiresInMinutes = 10
                    }
                };
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update exception during OTP generation for email: {Email}", otpRequest.EmailId);
                var sqlException = ex.InnerException as SqlException;
                var errorMessage = sqlException?.Message ?? ex.InnerException?.Message ?? ex.Message;

                return new AuthResponseDto
                {
                    Success = false,
                    Message = $"OTP generation failed: {errorMessage}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "General exception during OTP generation for email: {Email}", otpRequest.EmailId);
                return new AuthResponseDto
                {
                    Success = false,
                    Message = $"OTP generation failed: {ex.Message}"
                };
            }
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto loginRequest)
        {
            try
            {
                _logger.LogInformation("Starting login process for email: {Email}", loginRequest.EmailOrMobile);

                // Find user by email only (EmailOrMobile now contains only email)
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.EmailId == loginRequest.EmailOrMobile);

                if (user == null)
                {
                    _logger.LogWarning("Login failed: User not found with email {Email}", loginRequest.EmailOrMobile);
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Invalid email address or OTP"
                    };
                }

                _logger.LogDebug("User found for login: {UserId}", user.UserId);

                // Validate OTP
                var otpVerification = await _context.OtpVerifications
                    .FirstOrDefaultAsync(o => o.UserId == user.UserId &&
                                              o.OtpCode == loginRequest.OtpCode &&
                                              o.ExpiryTime > DateTime.Now);

                if (otpVerification == null)
                {
                    // Check if there was an OTP but it's expired
                    var expiredOtp = await _context.OtpVerifications
                        .FirstOrDefaultAsync(o => o.UserId == user.UserId && o.OtpCode == loginRequest.OtpCode);

                    var message = expiredOtp != null ? "OTP has expired. Please request a new one." : "Invalid OTP code";
                    _logger.LogWarning("OTP validation failed for user {UserId}: {Message}", user.UserId, message);

                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = message
                    };
                }

                _logger.LogDebug("OTP validation successful for user {UserId}", user.UserId);

                // Remove used OTP
                _context.OtpVerifications.Remove(otpVerification);

                // Update user's last login time
                user.UpdatedAt = DateTime.Now;
                _context.Users.Update(user);

                await _context.SaveChangesAsync();
                _logger.LogInformation("Login completed successfully for user {UserId}", user.UserId);

                // Generate JWT token with expiration
                var token = _jwtHelpers.GenerateToken(user.UserId.ToString(), user.EmailId, "User");
                _logger.LogDebug("JWT token generated successfully for user {UserId}", user.UserId);

                user.JWTtoken = token;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();


                return new AuthResponseDto
                {

                    Success = true,
                    Message = "Login successful",
                    //Token = token,
                    Token = token,

                    Data = new
                    {
                        UserId = user.UserId,
                        FullName = user.FullName,
                        EmailId = user.EmailId,
                        LastLoginAt = user.UpdatedAt,
                        TokenExpiration = DateTime.UtcNow.AddMinutes(60) // Match with JWT expiry
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed for email: {Email}", loginRequest.EmailOrMobile);
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Login failed: An error occurred during authentication"
                };
            }
        }

        public async Task<AuthResponseDto> GetNewOtpAsync(string emailAddress)
        {
            try
            {
                _logger.LogInformation("Generating new OTP for email: {Email}", emailAddress);

                // Find user by email only
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.EmailId == emailAddress);

                if (user == null)
                {
                    _logger.LogWarning("New OTP generation failed: User not found with email {Email}", emailAddress);
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "User not found with this email address"
                    };
                }

                var otpRequest = new GenerateOtpRequestDto
                {
                    FullName = user.FullName,
                    EmailId = user.EmailId
                };

                return await GenerateOtpAsync(otpRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate new OTP for email: {Email}", emailAddress);
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Failed to generate new OTP. Please try again."
                };
            }
        }

        public async Task<AuthResponseDto> ValidateTokenAsync(string token)
        {
            try
            {
                _logger.LogDebug("Starting token validation");

                var principal = _jwtHelpers.ValidateToken(token);
                var userIdClaim = principal.FindFirst("userId")?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    _logger.LogWarning("Token validation failed: Invalid user ID in token");
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Invalid token"
                    };
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
                if (user == null)
                {
                    _logger.LogWarning("Token validation failed: User not found for ID {UserId}", userId);
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "User not found"
                    };
                }

                _logger.LogDebug("Token validated successfully for user {UserId}", user.UserId);
                return new AuthResponseDto
                {
                    Success = true,
                    Message = "Token is valid",
                    Data = new
                    {
                        UserId = user.UserId,
                        FullName = user.FullName,
                        EmailId = user.EmailId
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Token validation failed");
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Invalid or expired token"
                };
            }
        }

        public async Task<AuthResponseDto> LogoutAsync(string token)
        {
            try
            {
                _logger.LogInformation("Processing logout request");

                // For JWT tokens, we can't really "invalidate" them on the server side
                // In a more sophisticated implementation, you might maintain a blacklist
                // For now, we'll just validate the token and return success

                var validationResult = await ValidateTokenAsync(token);
                if (!validationResult.Success)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Invalid token"
                    };
                }

                _logger.LogInformation("Logout successful");
                return new AuthResponseDto
                {
                    Success = true,
                    Message = "Logged out successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Logout failed");
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Logout failed"
                };
            }
        }

        // Private helper methods
        private string GenerateSecureOtp()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                var bytes = new byte[4];
                rng.GetBytes(bytes);
                var randomValue = Math.Abs(BitConverter.ToInt32(bytes, 0));
                return (randomValue % 900000 + 100000).ToString(); // Ensures 6-digit number
            }
        }

        private async Task SendRegistrationEmailAsync(User user)
        {
            var subject = "Welcome to DoctorateDrive!";
            var body = $"Hello {user.FullName}, Welcome to DoctorateDrive! Your account has been successfully created and you can now access our services.";
            await _emailService.SendEmailAsync(user.EmailId, subject, body);
        }

        private async Task SendOtpEmailAsync(User user, string otpCode)
        {
            var subject = "Your Login Code - DoctorateDrive";

            if (_emailService is EmailService emailService)
            {
                var emailBody = await emailService.GetOtpEmailBodyAsync(user.FullName, otpCode);
                await _emailService.SendEmailAsync(user.EmailId, subject, emailBody);
            }
            else
            {
                var body = $"Hello {user.FullName}, Your OTP code is: {otpCode}. This code expires in 10 minutes. Never share this code with anyone.";
                await _emailService.SendEmailAsync(user.EmailId, subject, body);
            }
        }
    }
}
