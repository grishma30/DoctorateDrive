using DoctorateDrive.Data;
using DoctorateDrive.DTOs;
using DoctorateDrive.Models;
using Microsoft.EntityFrameworkCore;

namespace DoctorateDrive.Services
{
    public class StudentService : IStudentService
    {
        private readonly DoctorateDriveContext _context;
        private readonly ILogger<StudentService> _logger;

        public StudentService(DoctorateDriveContext context, ILogger<StudentService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<string> SaveStudentDetailsAsync(StudentDetailsDto studentDto, int userId)
        {
            try
            {
                _logger.LogInformation("🚀 Starting to save student details for UserId: {UserId}", userId);

                // Generate Application ID
                var applicationId = GenerateApplicationId();
                _logger.LogInformation("📋 Generated Application ID: {ApplicationId}", applicationId);

                // Create StudentDetail entity
                var studentDetail = new StudentDetail
                {
                    UserId = userId,
                    FirstName = studentDto.FirstName,
                    MiddleName = studentDto.MiddleName ?? string.Empty,
                    LastName = studentDto.LastName,
                    DOB = studentDto.Dob,
                    Gender = studentDto.Gender,
                    MobileNumber = studentDto.MobileNumber,
                    CasteCategory = studentDto.CasteCategory,
                    Nationality = studentDto.Nationality,
                    CGPAGained = studentDto.CgpaInput,
                    CGPATotal = studentDto.TotalCgpa,
                    Percentage = studentDto.EquivalentPercentage,
                    GATEQualified = studentDto.GateSelect == "yes",
                    FeesPaid = "No",
                    WhatsappNumber = studentDto.WhatsappNumber,
                    EmailId = studentDto.Email,
                    Country = studentDto.Country,
                    State = studentDto.State,
                    City = studentDto.City,
                    PIN = "000000", // Default value
                    District = studentDto.District,
                    GuardianName = studentDto.GuardianName,
                    RelationWithGuardian = studentDto.RelationWithApplicant,
                    GuardianEmail = studentDto.GuardianEmail ?? string.Empty,
                    GuardianMobileNumber = studentDto.GuardianMobile ?? string.Empty,
                    Document = applicationId, // Store application ID
                    Pin = "000000", // Default value
                    Gatequalified = studentDto.GateSelect == "yes"
                };

                _logger.LogInformation("💾 Adding student details to context");
                _context.StudentDetails.Add(studentDetail);

                _logger.LogInformation("🔄 Calling SaveChangesAsync");
                await _context.SaveChangesAsync();

                _logger.LogInformation("✅ Student details saved with ID: {UserId}", studentDetail.UserId);

                return applicationId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Error saving student details for user {UserId}", userId);
                throw;
            }
        }

        public async Task<StudentDetailViewDto?> GetStudentDetailsByApplicationIdAsync(string applicationId)
        {
            try
            {
                var student = await _context.StudentDetails
                    .FirstOrDefaultAsync(s => s.Document == applicationId);

                if (student == null) return null;

                return new StudentDetailViewDto
                {
                    ApplicationId = applicationId,
                    FirstName = student.FirstName,
                    LastName = student.LastName,
                    Email = student.EmailId,
                    MobileNumber = student.MobileNumber,
                    CreatedDate = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving student details for Application ID: {ApplicationId}", applicationId);
                return null;
            }
        }

        private string GenerateApplicationId()
        {
            var year = DateTime.Now.Year.ToString().Substring(2, 2);
            var random = new Random().Next(100000, 999999).ToString();
            return $"{year}0A0AV{random.Substring(0, 3)}A1";
        }
    }
}
