using DoctorateDrive.Models;
using Microsoft.EntityFrameworkCore;

namespace DoctorateDrive.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly DoctorateDrive.Models.DoctorateDriveContext _context;

        public StudentRepository(DoctorateDrive.Models.DoctorateDriveContext context)
        {
            _context = context;
        }

        public async Task<StudentDetail> AddStudentDetailsAsync(StudentDetail student)
        {
             _context.StudentDetails.Add(student);
            await _context.SaveChangesAsync();
            return student;
        }
    }


    public interface IStudentRepository
    {
        Task<StudentDetail> AddStudentDetailsAsync(StudentDetail student);
    }

}
