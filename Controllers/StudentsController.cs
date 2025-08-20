using Microsoft.AspNetCore.Mvc;
using DoctorateDrive.Models;
using Microsoft.EntityFrameworkCore;

namespace DoctorateDrive.Controllers
{
    [ApiController]
    [Route("api/v1/student")]
    public class StudentsController : ControllerBase
    {
        private readonly DoctorateDriveContext _context;

        public StudentsController(DoctorateDriveContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateStudent([FromBody] StudentDetail student)
        {
            if (student == null)
                return BadRequest("Student data is null");

            student.CreatedAt = DateTime.UtcNow;
            student.UpdatedAt = DateTime.UtcNow;

            _context.StudentDetails.Add(student);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Student created successfully", student });
        }
    }
}
