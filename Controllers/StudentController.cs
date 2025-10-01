
using DoctorateDrive.Data;
using DoctorateDrive.DTOs;
using DoctorateDrive.Models;
using DoctorateDrive.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoctorateDrive.Controllers
{
    //[Authorize]
    public class StudentController : Controller
    {
        private readonly IStudentService _service;

        public StudentController(IStudentService service)
        {
            _service = service;
           
        }

        public async Task<IActionResult> Index()
        {
            var students = await _service.GetAllAsync();
            return View(students);
        }

        public async Task<IActionResult> Details(int id)
        {
            var student = await _service.GetByIdAsync(id);
            if (student == null) return NotFound();
            return View(student);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudentDetailDto dto)
        {
            if (ModelState.IsValid)
            {

                int userId = int.Parse(User.FindFirst("UserId").Value);
                // Check if mobile is verified
                var student = await _service.GetByUserIdAsync(userId); // implement this method in StudentService
                if (student != null && !student.IsMobileVerified)
                {
                    ModelState.AddModelError("", "Please verify your mobile number before submitting.");
                    return View(dto);
                }

                await _service.CreateStudentAsync(dto, userId);
                return RedirectToAction(nameof(Index));
            }
            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> SendOtp([FromForm] string mobileNumber)
        {
            int userId = int.Parse(User.FindFirst("UserId").Value);
            var otpCode = await _service.SendOtpAsync(userId, mobileNumber);
            return Ok(new { message = "OTP sent successfully." });
        }

        [HttpPost]
        public async Task<IActionResult> VerifyOtp([FromForm] string otpInput)
        {
            int userId = int.Parse(User.FindFirst("UserId").Value);
            bool isVerified = await _service.VerifyOtpAsync(userId, otpInput);

            if (isVerified)
                return Ok(new { message = "OTP verified successfully." });
            else
                return BadRequest(new { message = "Invalid or expired OTP." });
        }



        public async Task<IActionResult> Edit(int id)
        {
            var student = await _service.GetByIdAsync(id);
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
        public async Task<IActionResult> Edit(int id, StudentDetailDto dto)
        {
            if (ModelState.IsValid)
            {
                await _service.UpdateStudentAsync(id, dto);
                return RedirectToAction(nameof(Index));
            }
            return View(dto);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var student = await _service.GetByIdAsync(id);
            if (student == null) return NotFound();
            return View(student);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _service.DeleteStudentAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
