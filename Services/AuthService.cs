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
        private readonly DoctorateDrive.Helpers.IEmailService _emailService;  // ✅ Fully qualified - picks correct interface

        private readonly JWTHelpers _jwtHelpers;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            DoctorateDriveContext context,
            DoctorateDrive.Helpers.IEmailService emailService,
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
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _logger.LogInformation("Starting user registration process for email: {Email}", registerRequest.EmailId);

                // Check if user already exists
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

                // Create new user
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

                // Generate OTP immediately after user creation
                var otpCode = GenerateSecureOtp();
                _logger.LogDebug("OTP generated for user {UserId}", newUser.UserId);

                // Remove any existing OTPs for this user (cleanup)
                var existingOtps = _context.OtpVerifications
                    .Where(o => o.UserId == newUser.UserId);
                _context.OtpVerifications.RemoveRange(existingOtps);

                // Create new OTP record
                var otpVerification = new OtpVerification
                {
                    UserId = newUser.UserId,
                    OtpCode = otpCode,
                    ExpiryTime = DateTime.Now.AddMinutes(10),
                    CreatedAt = DateTime.Now
                };

                _context.OtpVerifications.Add(otpVerification);
                await _context.SaveChangesAsync();

                // Send OTP via email
                try
                {
                    await SendOtpEmailAsync(newUser, otpCode);
                    _logger.LogInformation("Registration OTP email sent successfully to user {UserId}", newUser.UserId);
                }
                catch (Exception emailEx)
                {
                    _logger.LogError(emailEx, "Failed to send registration OTP email for user {UserId}", newUser.UserId);

                    // Rollback transaction if email fails
                    await transaction.RollbackAsync();

                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Registration failed. Unable to send verification email. Please try again."
                    };
                }

                // Commit transaction
                await transaction.CommitAsync();

                return new AuthResponseDto
                {
                    Success = true,
                    Message = "Registration successful! OTP has been sent to your email. Please verify to complete registration.",
                    Data = new
                    {
                        UserId = newUser.UserId,
                        EmailId = newUser.EmailId,
                        OtpSent = true,
                        OtpExpiryMinutes = 10
                    }
                };
            }
            catch (DbUpdateException ex)
            {
                await transaction.RollbackAsync();
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
                await transaction.RollbackAsync();
                _logger.LogError(ex, "General error during registration for email: {Email}", registerRequest.EmailId);

                return new AuthResponseDto
                {
                    Success = false,
                    Message = $"Registration failed: {ex.Message}"
                };
            }
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto loginRequest)
        {
            try
            {
                _logger.LogInformation("Starting login process for email: {Email}", loginRequest.EmailOrMobile);

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

                // Validate OTP
                var otpVerification = await _context.OtpVerifications
                    .FirstOrDefaultAsync(o => o.UserId == user.UserId &&
                                              o.OtpCode == loginRequest.OtpCode &&
                                              o.ExpiryTime > DateTime.Now);

                if (otpVerification == null)
                {
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

                // Generate JWT token
                var token = _jwtHelpers.GenerateToken(user.UserId.ToString(), user.EmailId, "User");

                // Save token to user entity
                user.JWTtoken = token;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return new AuthResponseDto
                {
                    Success = true,
                    Message = "Login successful",
                    Token = token,
                    //JwtToken = token,
                    Data = new
                    {
                        UserId = user.UserId,
                        FullName = user.FullName,
                        EmailId = user.EmailId,
                        LastLoginAt = user.UpdatedAt,
                        TokenExpiration = DateTime.UtcNow.AddMinutes(60)
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

        public async Task<AuthResponseDto> ResendOtpAsync(string emailAddress)
        {
            try
            {
                _logger.LogInformation("Resending OTP for email: {Email}", emailAddress);

                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.EmailId == emailAddress);

                if (user == null)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "User not found with this email address"
                    };
                }

                // Check rate limiting (prevent OTP spam)
                var recentOtp = await _context.OtpVerifications
                    .Where(o => o.UserId == user.UserId)
                    .OrderByDescending(o => o.CreatedAt)
                    .FirstOrDefaultAsync();

                if (recentOtp != null && recentOtp.CreatedAt.AddMinutes(1) > DateTime.Now)
                {
                    var remainingSeconds = (int)(60 - (DateTime.Now - recentOtp.CreatedAt).TotalSeconds);
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = $"Please wait {remainingSeconds} seconds before requesting a new OTP"
                    };
                }

                // Generate new OTP
                var otpCode = GenerateSecureOtp();

                // Remove existing OTPs
                var existingOtps = _context.OtpVerifications.Where(o => o.UserId == user.UserId);
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
                await _context.SaveChangesAsync();

                // Send OTP via email
                await SendOtpEmailAsync(user, otpCode);

                return new AuthResponseDto
                {
                    Success = true,
                    Message = "New OTP has been sent to your email address",
                    Data = new
                    {
                        OtpExpiryMinutes = 10
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to resend OTP for email: {Email}", emailAddress);
                return new AuthResponseDto
                {
                    Success = false,
                    Message = "Failed to send OTP. Please try again."
                };
            }
        }

        public async Task<AuthResponseDto> ValidateTokenAsync(string token)
        {
            try
            {
                var principal = _jwtHelpers.ValidateToken(token);
                var userIdClaim = principal.FindFirst("userId")?.Value;

                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Invalid token"
                    };
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
                if (user == null)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "User not found"
                    };
                }

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
                var validationResult = await ValidateTokenAsync(token);
                if (!validationResult.Success)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Invalid token"
                    };
                }

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
                return (randomValue % 900000 + 100000).ToString();
            }
        }

        private async Task SendOtpEmailAsync(User user, string otpCode)
        {
            var subject = "Your Verification Code - DoctorateDrive";

            var body = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ 
            font-family: Arial, sans-serif; 
            background: #f4f4f4; 
            margin: 0; 
            padding: 20px; 
        }}
        .container {{ 
            max-width: 600px; 
            margin: 0 auto; 
            background: white; 
            border-radius: 10px; 
            overflow: hidden; 
            box-shadow: 0 4px 20px rgba(0,0,0,0.1); 
        }}
        .header {{ 
            background: linear-gradient(135deg, #1e5bb8 0%, #2980b9 100%); 
            padding: 30px; 
            text-align: center; 
            color: white; 
        }}
        .content {{ 
            padding: 40px 30px; 
        }}
        .otp-box {{ 
            background: #f8f9fa; 
            border: 2px dashed #1e5bb8; 
            border-radius: 10px; 
            padding: 20px; 
            text-align: center; 
            margin: 30px 0; 
        }}
        .otp-code {{ 
            font-size: 36px; 
            font-weight: bold; 
            color: #1e5bb8; 
            letter-spacing: 8px; 
        }}
        .cta-box {{
            background: #eef4ff;
            border-left: 4px solid #1e5bb8;
            padding: 15px;
            margin-top: 25px;
            border-radius: 8px;
        }}
        .cta-box a {{
            font-weight: bold;
            color: #1e5bb8;
            text-decoration: none;
            font-size: 16px;
        }}
        .footer {{ 
            background: #f8f9fa; 
            padding: 20px; 
            text-align: center; 
            font-size: 12px; 
            color: #6c757d; 
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🎓 DoctorateDrive</h1>
            <p>Email Verification Code</p>
        </div>

        <div class='content'>
            <h2>Hello {user.FullName},</h2>

            <p>
                Welcome to <strong>DoctorateDrive</strong>!  
                We're excited to have you join our platform.
            </p>

            <p>Please use the verification code below to complete your registration:</p>

            <div class='otp-box'>
                <div style='color: #6c757d; font-size: 14px; margin-bottom: 10px;'>Your OTP Code</div>
                <div class='otp-code'>{otpCode}</div>
                <div style='color: #6c757d; font-size: 12px; margin-top: 10px;'>Valid for 10 minutes</div>
            </div>

            <p style='color: #dc3545;'>⚠️ Never share this OTP with anyone.</p>

            <p>If you did not request this code, you can safely ignore this email.</p>

            <div class='cta-box'>
                Click here to start your application now:<br/><br/>
                <a href='#'>CHARUSAT Admission Application Portal</a>
            </div>

            <br/>

            <p><strong>Got questions? We're here to help!</strong></p>

            <p>
                Phone 1: 7863810274 <br/>
                Phone 2: 7863810275
            </p>

            <p>
                Regards,<br/>
                <strong>The Admissions Team</strong><br/>
                CHARUSAT<br/>
                www.charusat.ac.in
            </p>
        </div>

        <div class='footer'>
            <p>© 2025 DoctorateDrive. All rights reserved.</p>
            <p>This is a system-generated email. Please do not reply.</p>
        </div>
    </div>
</body>
</html>";

            await _emailService.SendEmailAsync(user.EmailId, subject, body);
        }
    }
}
