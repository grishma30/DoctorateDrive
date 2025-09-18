using DoctorateDrive.DTOs;
using DoctorateDrive.Models;
using System.Collections.Generic;

namespace DoctorateDrive.Services
{
    public interface IStudentService
    {
        IEnumerable<StudentDetail> GetAllStudents();
        StudentDetail? GetStudent(int id);
        void CreateStudent(StudentDetailDto dto, int userId);
        void UpdateStudent(int id, StudentDetailDto dto);
        void DeleteStudent(int id);
    }
}
