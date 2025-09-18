
using DoctorateDrive.DTOs;
using DoctorateDrive.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoctorateDrive.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {
        private readonly IStudentService _service;

        public StudentController(IStudentService service)
        {
            _service = service;
        }

        public IActionResult Index()
        {
            var students = _service.GetAllStudents();
            return View(students);
        }

        public IActionResult Details(int id)
        {
            var student = _service.GetStudent(id);
            if (student == null) return NotFound();
            return View(student);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(StudentDetailDto dto)
        {
            if (ModelState.IsValid)
            {
                // assume logged-in user's ID is stored in Claims
                int userId = int.Parse(User.FindFirst("UserId").Value);
                _service.CreateStudent(dto, userId);
                return RedirectToAction(nameof(Index));
            }
            return View(dto);
        }

        public IActionResult Edit(int id)
        {
            var student = _service.GetStudent(id);
            if (student == null) return NotFound();

            var dto = new StudentDetailDto
            {
                UserId = student.UserId,
                FirstName = student.FirstName,
                MiddleName = student.MiddleName,
                LastName = student.LastName,
                Dob = student.Dob,
                Gender = student.Gender,
                MobileNumber = student.MobileNumber,
                CasteCategory = student.CasteCategory,
                Nationality = student.Nationality,
                Cgpagained = student.Cgpagained,
                Cgpatotal = student.Cgpatotal,
                Percentage = student.Percentage,
                Gatequalified = student.Gatequalified,
                FeesPaid = student.FeesPaid,
                WhatsappNumber = student.WhatsappNumber,
                EmailId = student.EmailId,
                Country = student.Country,
                State = student.State,
                City = student.City,
                Pin = student.Pin,
                District = student.District,
                GuardianName = student.GuardianName,
                RelationWithGuardian = student.RelationWithGuardian,
                GuardianEmail = student.GuardianEmail,
                GuardianMobileNumber = student.GuardianMobileNumber
            };

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, StudentDetailDto dto)
        {
            if (ModelState.IsValid)
            {
                _service.UpdateStudent(id, dto);
                return RedirectToAction(nameof(Index));
            }
            return View(dto);
        }

        public IActionResult Delete(int id)
        {
            var student = _service.GetStudent(id);
            if (student == null) return NotFound();
            return View(student);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            _service.DeleteStudent(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
