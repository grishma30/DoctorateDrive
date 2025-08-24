using DoctorateDrive.Data;
using DoctorateDrive.DTOs;
using DoctorateDrive.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DoctorateDrive.Services
{
    public class AuthService : IAuthService
    {
        private readonly DoctorateDriveContext _context;

        public AuthService(DoctorateDriveContext context)
        {
            _context = context;
        }

        public async Task<AuthResponseDto> RegisterUserAsync(RegisterRequestDto registerRequest)
        {
            try
            {
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.EmailId == registerRequest.EmailId ||
                                              u.MobileNumber == registerRequest.MobileNumber);

                if (existingUser != null)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "User already exists with this email or mobile number"
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

                return new AuthResponseDto
                {
                    Success = true,
                    Message = "User registered successfully",
                    Data = new { UserId = newUser.UserId }
                };
            }
            catch (DbUpdateException ex)
            {
                // Enhanced error handling to capture inner exception details
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
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.EmailId == otpRequest.EmailId);

                if (user == null)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "User not found"
                    };
                }

                var random = new Random();
                var otpCode = random.Next(100000, 999999).ToString();

                // Remove existing OTPs for this user
                var existingOtps = _context.OtpVerifications
                    .Where(o => o.StudentID == user.UserId);

                _context.OtpVerifications.RemoveRange(existingOtps);

                // Create new OTP record
                var otpVerification = new OtpVerification
                {
                    StudentID = user.UserId,
                    OtpCode = otpCode,
                    ExpiryTime = DateTime.Now.AddMinutes(10),
                    CreatedAt = DateTime.Now
                };

                _context.OtpVerifications.Add(otpVerification);
                await _context.SaveChangesAsync();

                return new AuthResponseDto
                {
                    Success = true,
                    Message = "OTP generated successfully",
                    Data = new { OTP = otpCode, ExpiryTime = otpVerification.ExpiryTime }
                };
            }
            catch (DbUpdateException ex)
            {
                // Enhanced error handling to capture inner exception details
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
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.EmailId == loginRequest.EmailOrMobile ||
                                              u.MobileNumber == loginRequest.EmailOrMobile);

                if (user == null)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "User not found"
                    };
                }

                var otpVerification = await _context.OtpVerifications
                    .FirstOrDefaultAsync(o => o.StudentID == user.UserId &&
                                              o.OtpCode == loginRequest.OtpCode &&
                                              o.ExpiryTime > DateTime.Now);

                if (otpVerification == null)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Invalid or expired OTP"
                    };
                }

                _context.OtpVerifications.Remove(otpVerification);
                await _context.SaveChangesAsync();

                return new AuthResponseDto
                {
                    Success = true,
                    Message = "Login successful",
                    Data = new
                    {
                        UserId = user.UserId,
                        FullName = user.FullName,
                        EmailId = user.EmailId,
                        MobileNumber = user.MobileNumber
                    }
                };
            }
            catch (Exception ex)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = $"Login failed: {ex.Message}"
                };
            }
        }

        public async Task<AuthResponseDto> GetNewOtpAsync(string emailOrMobile)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.EmailId == emailOrMobile ||
                                              u.MobileNumber == emailOrMobile);

                if (user == null)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "User not found"
                    };
                }

                var otpRequest = new GenerateOtpRequestDto
                {
                    FullName = user.FullName,
                    EmailId = user.EmailId,
                    
                };

                return await GenerateOtpAsync(otpRequest);
            }
            catch (Exception ex)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = $"Failed to generate new OTP: {ex.Message}"
                };
            }
        }
    }
}
