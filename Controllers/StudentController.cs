using DoctorateDrive.Models;
using DoctorateDrive.Repositories;
using DoctorateDrive.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Xml.Linq;

namespace DoctorateDrive.Controllers
{
    public class StudentController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly ILogger<StudentController> _logger;
        private readonly IStudentRepository _studentRepository;
        private readonly IDocumentRepository _documentRepository;


        public StudentController(IStudentService studentService, ILogger<StudentController> logger, IDocumentRepository documentRepository)
        {
            _studentService = studentService;
            _logger = logger;
            _documentRepository = documentRepository;
        }


        [HttpPost]
        //[ValidateAntiForgeryToken] // helps prevent CSRF attacks
        public async Task<IActionResult> Add(StudentDetail student, IFormFile document, string documentType)
        {
            if (!ModelState.IsValid)
            {
                // log validation issue
                _logger.LogWarning("Student model validation failed for {EmailId}", student.EmailId);
                return View(student);
            }


            try
            {
                var result = await _studentService.AddStudentDetailsAsync(student);
                // ✅ Handle file upload
                if (document != null && document.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + document.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await document.CopyToAsync(stream);
                    }

                    // ✅ Create a new Document entry
                    var newDocument = new Document
                    {
                        StudentId = result.StudentId,
                        DocumentType = documentType, // e.g., "Transcript", "ID Proof"
                        FilePath = "/uploads/" + uniqueFileName,
                        UploadedDate = DateTime.UtcNow
                    };

                    // save document (you may want a DocumentService/Repository for this)
                    //_studentRepository.Documents.Add(newDocument);
                    //await _studentRepository.SaveChangesAsync();
                    await _documentRepository.AddDocumentAsync(newDocument);
                }


                TempData["SuccessMessage"] = "Student details and document added successfully!";
                return RedirectToAction("Index"); // redirect after successful save
            }
            catch (Exception ex)
            {
                // log the actual exception
                _logger.LogError(ex, "Error occurred while adding student details for {EmailId}", student.EmailId);

                // Show user-friendly error
                TempData["ErrorMessage"] = "Something went wrong while saving student details. Please try again.";
                return View(student);
            }
        }

        // optional - to display form
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }
    }
}
