using DoctorateDrive.DTOs;
using DoctorateDrive.Models;
using System.Collections.Generic;

namespace DoctorateDrive.Services
{
    public interface IStudentService
    {
        ////IEnumerable<StudentDetail> GetAllStudents();
        ////StudentDetail? GetStudent(int id);
        ////void CreateStudent(StudentDetailDto dto, int userId);
        ////void UpdateStudent(int id, StudentDetailDto dto);
        ////void DeleteStudent(int id);

        //Task<bool> CreateStudentAsync(StudentDetailDto studentDto);
        //Task<IEnumerable<StudentDetailDto>> GetAllStudentsAsync();

        //Task<IEnumerable<StudentDetail>> GetAllStudentsAsync();
        Task<IEnumerable<StudentDetail>> GetAllAsync();
        //Task<StudentDetail?> GetStudentByIdAsync(int id);
        Task<StudentDetail?> GetByIdAsync(int id);
        //Task CreateStudentAsync(StudentDetailDto dto, int userId);
        Task CreateStudentAsync(StudentDetailDto dto, int userId);
        Task UpdateStudentAsync(int id, StudentDetailDto dto);
        Task DeleteStudentAsync(int id);
        Task<StudentDetail?> GetByUserIdAsync(int userId);
        // OTP methods
        Task<string> SendOtpAsync(int userId, string mobileNumber); // generates OTP
        Task<bool> VerifyOtpAsync(int userId, string otpCode); // verifies OTP

    }
}
