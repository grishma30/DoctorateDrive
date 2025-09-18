
using DoctorateDrive.Models;
using DoctorateDrive.Data;

using System.Collections.Generic;

namespace DoctorateDrive.Repositories
{
    public interface IStudentRepository
    {
        IEnumerable<StudentDetail> GetAll();
        StudentDetail? GetById(int id);
        void Add(StudentDetail student);
        void Update(StudentDetail student);
        void Delete(int id);
    }
}
