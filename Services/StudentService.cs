using DoctorateDrive.Models;
using DoctorateDrive.Repositories;

namespace DoctorateDrive.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;

        public StudentService(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public async Task<StudentDetail> AddStudentDetailsAsync(StudentDetail student)
        {
            // Business rules can go here (validation, defaults, etc.)
            student.CreatedAt = DateTime.Now;
            student.UpdatedAt = DateTime.Now;

            return await _studentRepository.AddStudentDetailsAsync(student);
        }
    }

    public interface IStudentService
    {
        Task<StudentDetail> AddStudentDetailsAsync(StudentDetail student);
    }
}
