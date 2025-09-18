using DoctorateDrive.Models;
using DoctorateDrive.Data;

using System.Collections.Generic;
using System.Linq;

namespace DoctorateDrive.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly DoctorateDrive.Data.DoctorateDriveContext _context;

        public StudentRepository(DoctorateDrive.Data.DoctorateDriveContext context)
        {
            _context = context;
        }

        public IEnumerable<StudentDetail> GetAll() => _context.StudentDetails.ToList();

        public StudentDetail? GetById(int id) => _context.StudentDetails.FirstOrDefault(s => s.StudentId == id);

        public void Add(StudentDetail student)
        {
            _context.StudentDetails.Add(student);
            _context.SaveChanges();
        }

        public void Update(StudentDetail student)
        {
            _context.StudentDetails.Update(student);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var student = _context.StudentDetails.FirstOrDefault(s => s.StudentId == id);
            if (student != null)
            {
                _context.StudentDetails.Remove(student);
                _context.SaveChanges();
            }
        }
    }
}
