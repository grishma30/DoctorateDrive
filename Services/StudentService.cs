using DoctorateDrive.Data;
using DoctorateDrive.DTOs;
using DoctorateDrive.Models;
using DoctorateDrive.Repositories;
using DoctorateDrive.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;


namespace DoctorateDrive.Services
{
    public class StudentService : IStudentService
    {
        //private readonly IStudentRepository _repository;

        //public StudentService(IStudentRepository repository)
        //{
        //    _repository = repository;
        //}
        private readonly DoctorateDriveContext _context;

        public StudentService(DoctorateDriveContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StudentDetail>> GetAllAsync()
        {
            return await _context.StudentDetails.ToListAsync();
        }


        public async Task<StudentDetail?> GetByIdAsync(int id)
        {
            return await _context.StudentDetails.FindAsync(id);
        }

        public async Task CreateStudentAsync(StudentDetailDto dto, int userId)
        {
            // Generate ApplicationId
            var year = DateTime.Now.Year;
            var lastStudent = await _context.StudentDetails
                .OrderByDescending(s => s.StudentId)
                .FirstOrDefaultAsync();

            int nextNumber = 1;
            if (lastStudent != null && !string.IsNullOrEmpty(lastStudent.ApplicationId))
            {
                // Extract last number from ApplicationId
                var lastNumberStr = lastStudent.ApplicationId.Split('-').Last();
                int.TryParse(lastNumberStr, out int lastNumber);
                nextNumber = lastNumber + 1;
            }

            string applicationId = $"APP{year}-{nextNumber:0000}";

            var student = new StudentDetail
            {
                UserId = userId,
                ApplicationId = applicationId,
                FirstName = dto.FirstName,
                MiddleName = dto.MiddleName,
                LastName = dto.LastName,
                Dob = dto.Dob,
                Gender = dto.Gender,
                MobileNumber = dto.MobileNumber,
                CasteCategory = dto.CasteCategory,
                Nationality = dto.Nationality,
                Cgpagained = dto.Cgpagained,
                Cgpatotal = dto.Cgpatotal,
                Percentage = dto.Percentage,
                Gatequalified = dto.Gatequalified,
                FeesPaid = dto.FeesPaid,
                WhatsappNumber = dto.WhatsappNumber,
                EmailId = dto.EmailId,
                Country = dto.Country,
                State = dto.State,
                City = dto.City,
                Pin = dto.Pin,
                District = dto.District,
                GuardianName = dto.GuardianName,
                RelationWithGuardian = dto.RelationWithGuardian,
                GuardianEmail = dto.GuardianEmail,
                GuardianMobileNumber = dto.GuardianMobileNumber,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };


            _context.StudentDetails.Add(student);
            await _context.SaveChangesAsync();

            // 2️⃣ Save documents
            if (dto.Documents != null)
            {
                var files = new List<(IFormFile file, string type)>
        {
            (dto.Documents.GraduateCertificate, "GraduateCertificate"),
            (dto.Documents.PostGraduateCertificate, "PostGraduateCertificate"),
            (dto.Documents.GateCertificate, "GateCertificate"),
            (dto.Documents.ConversionCertificate, "ConversionCertificate")
        };

                foreach (var (file, type) in files)
                {
                    if (file != null && file.Length > 0)
                    {
                        var filePath = await SaveFileAsync(file);

                        var document = new Document
                        {
                            StudentId = student.StudentId,
                            DocumentType = type,
                            FilePath = filePath,
                            UploadedDate = DateTime.Now
                        };

                        _context.Documents.Add(document);
                    }
                }

                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateStudentAsync(int id, StudentDetailDto dto)
        {
            var existing = await _context.StudentDetails.FindAsync(id);
            if (existing != null)
            {
                existing.FirstName = dto.FirstName;
                existing.MiddleName = dto.MiddleName;
                existing.LastName = dto.LastName;
                existing.Dob = dto.Dob;
                existing.Gender = dto.Gender;
                existing.MobileNumber = dto.MobileNumber;
                existing.CasteCategory = dto.CasteCategory;
                existing.Nationality = dto.Nationality;
                existing.Cgpagained = dto.Cgpagained;
                existing.Cgpatotal = dto.Cgpatotal;
                existing.Percentage = dto.Percentage;
                existing.Gatequalified = dto.Gatequalified;
                existing.FeesPaid = dto.FeesPaid;
                existing.WhatsappNumber = dto.WhatsappNumber;
                existing.EmailId = dto.EmailId;
                existing.Country = dto.Country;
                existing.State = dto.State;
                existing.City = dto.City;
                existing.Pin = dto.Pin;
                existing.District = dto.District;
                existing.GuardianName = dto.GuardianName;
                existing.RelationWithGuardian = dto.RelationWithGuardian;
                existing.GuardianEmail = dto.GuardianEmail;
                existing.GuardianMobileNumber = dto.GuardianMobileNumber;
                existing.UpdatedAt = DateTime.Now;

                // Optionally handle document updates here

                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteStudentAsync(int id)
        {
            var student = await _context.StudentDetails.FindAsync(id);
            if (student != null)
            {
                _context.StudentDetails.Remove(student);
                await _context.SaveChangesAsync();
            }
        }

        // Helper to save uploaded files
        private async Task<string?> SaveFileAsync(IFormFile? file)
        {
            if (file == null || file.Length == 0)
                return null;

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var filePath = Path.Combine(uploadsFolder, Guid.NewGuid() + Path.GetExtension(file.FileName));
            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            // Return relative path
            return filePath.Replace(Directory.GetCurrentDirectory(), "").Replace("\\", "/");
        }

        public async Task<OtpVerification> SendOtpAsync(int userId)
        {
            var otp = new OtpVerification
            {
                UserId = userId,
                OtpCode = new Random().Next(100000, 999999).ToString(),
                CreatedAt = DateTime.Now,
                ExpiryTime = DateTime.Now.AddMinutes(5)
            };

            _context.OtpVerifications.Add(otp);
            await _context.SaveChangesAsync();
            return otp;
        }


        public async Task<StudentDetail?> GetByUserIdAsync(int userId)
        {
            return await _context.StudentDetails
                                 .FirstOrDefaultAsync(s => s.UserId == userId);
        }


        public async Task<string> SendOtpAsync(int userId, string mobileNumber)
        {
            // Generate OTP
            var otpCode = new Random().Next(100000, 999999).ToString();

            var otp = new OtpVerification
            {
                UserId = userId,
                OtpCode = otpCode,
                CreatedAt = DateTime.Now,
                ExpiryTime = DateTime.Now.AddMinutes(5)
            };

            _context.OtpVerifications.Add(otp);
            await _context.SaveChangesAsync();

            // Here you would integrate with SMS gateway to actually send OTP
            return otpCode;
        }

        public async Task<bool> VerifyOtpAsync(int userId, string otpCode)
        {
            var otp = await _context.OtpVerifications
                .Where(o => o.UserId == userId && o.OtpCode == otpCode)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefaultAsync();

            if (otp == null) return false;
            if (otp.ExpiryTime < DateTime.Now) return false;

            // Mark student as verified
            var student = await _context.StudentDetails.FirstOrDefaultAsync(s => s.UserId == userId);
            if (student != null)
            {
                student.IsMobileVerified = true;
                await _context.SaveChangesAsync();
            }

            return true;
        }

        //[HttpPost]
        //public async Task<IActionResult> SendOtp()
        //{
        //    int userId = int.Parse(User.FindFirst("UserId").Value);
        //    var mobileNumber = "..."; // get from form input
        //    var otpCode = await _service.SendOtpAsync(userId, mobileNumber);

        //    // Return success (or JSON for AJAX)
        //    return Ok(new { message = "OTP sent successfully." });
        //}

        //[HttpPost]
        //public async Task<IActionResult> VerifyOtp(string otpInput)
        //{
        //    int userId = int.Parse(User.FindFirst("UserId").Value);
        //    bool isVerified = await _context.VerifyOtpAsync(userId, otpInput);

        //    if (isVerified)
        //        return Ok(new { message = "OTP verified successfully." });
        //    else
        //        return _context(new { message = "Invalid or expired OTP." });
        }


        //public IEnumerable<StudentDetail> GetAllStudents() => _repository.GetAll();

        //public StudentDetail? GetStudent(int id) => _repository.GetById(id);

        //public void CreateStudent(StudentDetailDto dto, int userId)
        //{
        //    var student = new StudentDetail
        //    {
        //        UserId = userId,
        //        FirstName = dto.FirstName,
        //        MiddleName = dto.MiddleName,
        //        LastName = dto.LastName,
        //        Dob = dto.Dob,
        //        Gender = dto.Gender,
        //        MobileNumber = dto.MobileNumber,
        //        CasteCategory = dto.CasteCategory,
        //        Nationality = dto.Nationality,
        //        Cgpagained = dto.Cgpagained,
        //        Cgpatotal = dto.Cgpatotal,
        //        Percentage = dto.Percentage,
        //        Gatequalified = dto.Gatequalified,
        //        FeesPaid = dto.FeesPaid,
        //        WhatsappNumber = dto.WhatsappNumber,
        //        EmailId = dto.EmailId,
        //        Country = dto.Country,
        //        State = dto.State,
        //        City = dto.City,
        //        Pin = dto.Pin,
        //        District = dto.District,
        //        GuardianName = dto.GuardianName,
        //        RelationWithGuardian = dto.RelationWithGuardian,
        //        GuardianEmail = dto.GuardianEmail,
        //        GuardianMobileNumber = dto.GuardianMobileNumber,
        //        CreatedAt = DateTime.Now,
        //        UpdatedAt = DateTime.Now
        //    };

        //    _repository.Add(student);
        //}

        //public void UpdateStudent(int id, StudentDetailDto dto)
        //{
        //    var existing = _repository.GetById(id);
        //    if (existing != null)
        //    {
        //        existing.FirstName = dto.FirstName;
        //        existing.MiddleName = dto.MiddleName;
        //        existing.LastName = dto.LastName;
        //        existing.Dob = dto.Dob;
        //        existing.Gender = dto.Gender;
        //        existing.MobileNumber = dto.MobileNumber;
        //        existing.CasteCategory = dto.CasteCategory;
        //        existing.Nationality = dto.Nationality;
        //        existing.Cgpagained = dto.Cgpagained;
        //        existing.Cgpatotal = dto.Cgpatotal;
        //        existing.Percentage = dto.Percentage;
        //        existing.Gatequalified = dto.Gatequalified;
        //        existing.FeesPaid = dto.FeesPaid;
        //        existing.WhatsappNumber = dto.WhatsappNumber;
        //        existing.EmailId = dto.EmailId;
        //        existing.Country = dto.Country;
        //        existing.State = dto.State;
        //        existing.City = dto.City;
        //        existing.Pin = dto.Pin;
        //        existing.District = dto.District;
        //        existing.GuardianName = dto.GuardianName;
        //        existing.RelationWithGuardian = dto.RelationWithGuardian;
        //        existing.GuardianEmail = dto.GuardianEmail;
        //        existing.GuardianMobileNumber = dto.GuardianMobileNumber;
        //        existing.UpdatedAt = DateTime.Now;

        //        _repository.Update(existing);
        //    }
        //}

        //public void DeleteStudent(int id) => _repository.Delete(id);


    }
}
