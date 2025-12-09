using DoctorateDrive.Data;
using DoctorateDrive.DTOs;
using DoctorateDrive.Models;
using DoctorateDrive.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO; 
using System.Linq;

namespace DoctorateDrive.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ILogger<HomeController> _logger;
        private readonly IStudentService _studentService;
        private readonly DoctorateDriveContext _context;
        private readonly DoctorateDrive.Helpers.IEmailService _emailService;
        private readonly IPaymentService _paymentService;
        private readonly IConfiguration _configuration;

        public HomeController(IAuthService authService,
                             ILogger<HomeController> logger,
                             IStudentService studentService,
                            DoctorateDrive.Helpers.IEmailService emailService,
                               IConfiguration configuration,
                             IPaymentService paymentService,
                             DoctorateDriveContext context)
        {
            _authService = authService;
            _logger = logger;
            _studentService = studentService;
            _context = context;
            _emailService = emailService;
            _configuration = configuration;  
            _paymentService = paymentService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequestDto model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.FullName) || string.IsNullOrEmpty(model.EmailId))
                {
                    ViewBag.ErrorMessage = "Please fill in all required fields.";
                    return View(model);
                }

                var result = await _authService.RegisterUserAsync(model);

                if (result.Success)
                {
                    TempData["RegistrationSuccess"] = true;
                    TempData["UserName"] = model.FullName;
                    TempData["UserEmail"] = model.EmailId;
                    TempData["SuccessMessage"] = result.Message;
                    return RedirectToAction("RegisterSuccess");
                }
                else
                {
                    ViewBag.ErrorMessage = result.Message;
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration");
                ViewBag.ErrorMessage = "An error occurred during registration: " + ex.Message;
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult RegisterSuccess()
        {
            if (TempData["RegistrationSuccess"] == null)
            {
                return RedirectToAction("Register");
            }

            ViewBag.UserName = TempData["UserName"];
            ViewBag.UserEmail = TempData["UserEmail"];
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.EmailOrMobile) || string.IsNullOrEmpty(model.OtpCode))
                {
                    ViewBag.ErrorMessage = "Please fill in all required fields.";
                    return View(model);
                }

                var result = await _authService.LoginAsync(model);

                if (result.Success)
                {
                    HttpContext.Session.SetString("JwtToken", result.Token ?? "");
                    HttpContext.Session.SetString("UserEmail", model.EmailOrMobile);
                    return RedirectToAction("StudentDetails");
                }
                else
                {
                    ViewBag.ErrorMessage = result.Message;
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                ViewBag.ErrorMessage = "An error occurred during login: " + ex.Message;
                return View(model);
            }
        }

        [HttpGet]  // Changed from HttpPost to HttpGet
        public IActionResult Logout()
        {
            // Clear JWT token (if stored server-side)
            HttpContext.Session.Clear();

            // Redirect to login
            return RedirectToAction("Login", "Home");
        }

        public IActionResult Payment()
        {
            return View();
        }



        [HttpGet]
        public async Task<IActionResult> StudentDetails()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login");
            }

            var userEmail = HttpContext.Session.GetString("UserEmail");
            var user = await _context.Users.FirstOrDefaultAsync(u => u.EmailId == userEmail);

            if (user != null)
            {
                // ✅ CHECK IF STUDENT DETAILS ALREADY EXIST
                var existingStudent = await _context.StudentDetails
                    .FirstOrDefaultAsync(s => s.UserId == user.UserId);

                if (existingStudent != null)
                {
                    // Student details already exist - redirect to ApplicationId page
                    _logger.LogInformation("Student details already exist. Redirecting to ApplicationId page.");
                    return RedirectToAction("ApplicationId", new { id = existingStudent.Document });
                }
            }

            ViewBag.UserEmail = userEmail;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StudentDetails(IFormCollection form)
        {
            try
            {
                _logger.LogInformation("=== StudentDetails POST Started ===");

                var userEmail = HttpContext.Session.GetString("UserEmail");
                if (string.IsNullOrEmpty(userEmail))
                    return RedirectToAction("Login");

                var user = await _context.Users.FirstOrDefaultAsync(u => u.EmailId == userEmail);
                if (user == null)
                    return RedirectToAction("Login");

                _logger.LogInformation("User found: UserId={UserId}", user.UserId);

                // ✅ CHECK IF STUDENT ALREADY EXISTS (using UserId as primary key)
                var existingStudent = await _context.StudentDetails
                    .FirstOrDefaultAsync(s => s.UserId == user.UserId);

                // Parse form data
                decimal cgpa = 0, totalCgpa = 0, percentage = 0;
                decimal.TryParse(form["cgpaInput"], out cgpa);
                decimal.TryParse(form["totalCgpa"], out totalCgpa);
                decimal.TryParse(form["equivalentPercentage"], out percentage);

                // ❗ Eligibility check – minimum CGPA required is 6.0
                if (cgpa < 6)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "You are not eligible to apply. Minimum CGPA required is 6.0."
                    });
                }


                DateTime dob = DateTime.Now.AddYears(-25);
                DateTime.TryParse(form["dob"], out dob);

                bool isGateQualified = form["gateSelect"].ToString().ToLower() == "yes";
                string preferredInstitute = form["preferredInstitute"].ToString().Trim();
                string preferredSpecialization = form["preferredSpecialization"].ToString().Trim();

                string appId = existingStudent?.Document ?? await GenerateAppIdAsync();

                if (existingStudent != null)
                {
                    // ✅ UPDATE EXISTING RECORD
                    _logger.LogInformation("UPDATING existing student for UserId={UserId}", user.UserId);

                    existingStudent.FirstName = form["firstName"].ToString().Trim();
                    existingStudent.MiddleName = form["middleName"].ToString().Trim();
                    existingStudent.LastName = form["lastName"].ToString().Trim();
                    existingStudent.EmailId = form["email"].ToString().Trim();
                    existingStudent.MobileNumber = form["mobileNumber"].ToString().Trim();
                    existingStudent.WhatsappNumber = form["whatsappNumber"].ToString().Trim();
                    existingStudent.DOB = dob;
                    existingStudent.Gender = form["gender"].ToString().Trim();
                    existingStudent.CasteCategory = form["casteCategory"].ToString().Trim();
                    existingStudent.Nationality = form["nationality"].ToString().Trim();
                    existingStudent.CGPAGained = cgpa;
                    existingStudent.CGPATotal = totalCgpa;
                    existingStudent.Percentage = percentage;
                    existingStudent.GATEQualified = isGateQualified;
                    existingStudent.GraduateQualification = form["graduateSelect"].ToString().Trim();
                    existingStudent.PostGraduateQualification = form["postGraduateSelect"].ToString().Trim();
                    existingStudent.Country = form["country"].ToString().Trim();
                    existingStudent.State = form["state"].ToString().Trim();
                    existingStudent.City = form["city"].ToString().Trim();
                    existingStudent.District = form["district"].ToString().Trim();
                    existingStudent.PIN = form["pin"].ToString().Trim();
                    existingStudent.GuardianName = form["guardianName"].ToString().Trim();
                    existingStudent.RelationWithGuardian = form["relationWithApplicant"].ToString().Trim();
                    existingStudent.GuardianEmail = form["guardianEmail"].ToString().Trim();
                    existingStudent.GuardianMobileNumber = form["guardianMobile"].ToString().Trim();
                    existingStudent.PreferredInstitute = preferredInstitute;
                    existingStudent.PreferredSpecialization = preferredSpecialization;
                    existingStudent.UpdatedAt = DateTime.Now;

                    _context.StudentDetails.Update(existingStudent);
                    await _context.SaveChangesAsync();

                    await HandleFileUploads(Request.Form.Files, existingStudent, appId);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("✅ Student UPDATED! UserId={UserId}, AppId={AppId}", user.UserId, appId);
                }
                else
                {
                    // ✅ INSERT NEW RECORD
                    _logger.LogInformation("CREATING new student for UserId={UserId}", user.UserId);

                    var student = new StudentDetail
                    {
                        UserId = user.UserId,  // ✅ This is the primary key
                        FirstName = form["firstName"].ToString().Trim(),
                        MiddleName = form["middleName"].ToString().Trim(),
                        LastName = form["lastName"].ToString().Trim(),
                        EmailId = form["email"].ToString().Trim(),
                        MobileNumber = form["mobileNumber"].ToString().Trim(),
                        WhatsappNumber = form["whatsappNumber"].ToString().Trim(),
                        DOB = dob,
                        Gender = form["gender"].ToString().Trim(),
                        CasteCategory = form["casteCategory"].ToString().Trim(),
                        Nationality = form["nationality"].ToString().Trim(),
                        CGPAGained = cgpa,
                        CGPATotal = totalCgpa,
                        Percentage = percentage,
                        GATEQualified = isGateQualified,
                        GraduateQualification = form["graduateSelect"].ToString().Trim(),
                        PostGraduateQualification = form["postGraduateSelect"].ToString().Trim(),
                        Country = form["country"].ToString().Trim(),
                        State = form["state"].ToString().Trim(),
                        City = form["city"].ToString().Trim(),
                        District = form["district"].ToString().Trim(),
                        PIN = form["pin"].ToString().Trim(),
                        GuardianName = form["guardianName"].ToString().Trim(),
                        RelationWithGuardian = form["relationWithApplicant"].ToString().Trim(),
                        GuardianEmail = form["guardianEmail"].ToString().Trim(),
                        GuardianMobileNumber = form["guardianMobile"].ToString().Trim(),
                        Document = appId,
                        FeesPaid = "No",
                        PreferredInstitute = preferredInstitute,
                        PreferredSpecialization = preferredSpecialization,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    _context.StudentDetails.Add(student);
                    await _context.SaveChangesAsync();

                    await HandleFileUploads(Request.Form.Files, student, appId);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("✅ Student CREATED! UserId={UserId}, AppId={AppId}", user.UserId, appId);
                }

                return RedirectToAction("ApplicationId", new { id = appId });
            }
            catch (DbUpdateException dbEx)
            {
                var innerError = dbEx.InnerException?.Message ?? dbEx.Message;
                _logger.LogError(dbEx, "❌ Database error: {Error}", innerError);
                ViewBag.ErrorMessage = $"Database error: {innerError}";
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error: {Error}", ex.Message);
                ViewBag.ErrorMessage = $"Error: {ex.Message}";
                return View();
            }
        }
//        DD-202502-00001-ABX9
//➡️ Prefix(DD)
//➡️ Year(2025)
//➡️ Month(02)
//➡️ Sequence(00001)
//➡️ Random 4-character code(A–Z, 0–9)
        private async Task<string> GenerateAppIdAsync()
        {
            var now = DateTime.Now;

            string year = now.Year.ToString();         // 2025
            string month = now.Month.ToString("D2");   // 02

            int currentYear = now.Year;
            int currentMonth = now.Month;

            int sequenceNumber = 0;

            // SERIALIZABLE = highest safety, prevents duplicate IDs
            using (var transaction = await _context.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable))
            {
                // Count rows for THIS year+month inside the transaction
                int count = await _context.StudentDetails
                    .CountAsync(s => s.CreatedAt.Year == currentYear &&
                                     s.CreatedAt.Month == currentMonth);

                sequenceNumber = count + 1;  // Next safe sequence

                await transaction.CommitAsync(); // Release lock
            }

            // Sequence - 4 digits → 0001, 0203, etc.
            string sequence = sequenceNumber.ToString("D4");

            // Generate 4 random characters
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            string randomCode = new string(Enumerable.Repeat(chars, 4)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            // Final ID
            string applicationId = $"DD-{year}{month}-{sequence}-{randomCode}";

            return applicationId;
        }

        //private string GenerateAppId()
        //{
        //    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        //    var random = new Random();
        //    return new string(Enumerable.Repeat(chars, 9)
        //        .Select(s => s[random.Next(s.Length)]).ToArray());
        //}



        // ============================================
        // FILE UPLOAD HELPER METHOD
        // ============================================
        private async Task HandleFileUploads(IFormFileCollection files, StudentDetail student, string appId)
        {
            try
            {
                // Create upload directory if it doesn't exist
                var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "certificates");
                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                    _logger.LogInformation("Created uploads directory: {Path}", uploadsPath);
                }

                // Graduate Certificate
                var graduateCert = files.FirstOrDefault(f => f.Name == "graduateCertificate");
                if (graduateCert != null && graduateCert.Length > 0)
                {
                    var fileName = $"{appId}_Graduate_{Path.GetExtension(graduateCert.FileName)}";
                    var filePath = Path.Combine(uploadsPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await graduateCert.CopyToAsync(stream);
                    }

                    student.GraduateCertificatePath = $"/uploads/certificates/{fileName}";
                    _logger.LogInformation("✅ Graduate certificate uploaded: {FileName}", fileName);
                }

                // Post Graduate Certificate
                var pgCert = files.FirstOrDefault(f => f.Name == "postGraduateCertificate");
                if (pgCert != null && pgCert.Length > 0)
                {
                    var fileName = $"{appId}_PostGraduate_{Path.GetExtension(pgCert.FileName)}";
                    var filePath = Path.Combine(uploadsPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await pgCert.CopyToAsync(stream);
                    }

                    student.PostGraduateCertificatePath = $"/uploads/certificates/{fileName}";
                    _logger.LogInformation("✅ PG certificate uploaded: {FileName}", fileName);
                }

                // GATE Certificate
                var gateCert = files.FirstOrDefault(f => f.Name == "gateCertificate");
                if (gateCert != null && gateCert.Length > 0)
                {
                    var fileName = $"{appId}_GATE_{Path.GetExtension(gateCert.FileName)}";
                    var filePath = Path.Combine(uploadsPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await gateCert.CopyToAsync(stream);
                    }

                    student.GateCertificatePath = $"/uploads/certificates/{fileName}";
                    _logger.LogInformation("✅ GATE certificate uploaded: {FileName}", fileName);
                }

                // Conversion Certificate
                var conversionCert = files.FirstOrDefault(f => f.Name == "conversionCertificate");
                if (conversionCert != null && conversionCert.Length > 0)
                {
                    var fileName = $"{appId}_Conversion_{Path.GetExtension(conversionCert.FileName)}";
                    var filePath = Path.Combine(uploadsPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await conversionCert.CopyToAsync(stream);
                    }

                    // Note: You'll need to add this column to StudentDetail model if you want to store it
                    _logger.LogInformation("✅ Conversion certificate uploaded: {FileName}", fileName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error uploading files");
                // Don't throw - allow form submission to continue even if file upload fails
            }
        }




        // Add this method at the end of your HomeController class
        

        [HttpGet]
        public async Task<IActionResult> ApplicationId(string id)
        {
            _logger.LogInformation("=== ApplicationId GET called with id={Id} ===", id);

            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login");
            }

            StudentDetail student;

            if (string.IsNullOrEmpty(id))
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.EmailId == userEmail);
                if (user == null)
                {
                    return RedirectToAction("Login");
                }

                student = await _context.StudentDetails
                    .FirstOrDefaultAsync(s => s.UserId == user.UserId);
            }
            else
            {
                student = await _context.StudentDetails
                    .FirstOrDefaultAsync(s => s.Document == id);
            }

            if (student == null)
            {
                _logger.LogWarning("Student not found");
                return RedirectToAction("StudentDetails");
            }

            var model = new StudentApplicationViewModel
            {
                ApplicationId = student.Document,
                FullName = $"{student.FirstName} {student.LastName}",
                Email = student.EmailId,
                MobileNumber = student.MobileNumber
            };

            _logger.LogInformation("Showing ApplicationId page for: {Name}", model.FullName);

            return View(model);
        }



        // Payment Page
        [HttpGet]
        public async Task<IActionResult> Payment(string id)
        {
            var userEmail = HttpContext.Session.GetString("UserEmail");
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login");
            }

            var student = await _context.StudentDetails
                .FirstOrDefaultAsync(s => s.Document == id);

            if (student == null)
            {
                return RedirectToAction("ApplicationId");
            }

            var model = new PaymentViewModel
            {
                ApplicationId = id,
                StudentName = $"{student.FirstName} {student.LastName}",
                Email = student.EmailId,
                Mobile = student.MobileNumber,
                Amount = 1000, // Set your amount here
                KeyId = _configuration["RazorpaySettings:KeyId"],
                CompanyName = _configuration["RazorpaySettings:CompanyName"],
                CompanyLogo = _configuration["RazorpaySettings:CompanyLogo"]
            };

            return View(model);
        }

        // Create Razorpay Order
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] decimal amount)
        {
            try
            {
                var userEmail = HttpContext.Session.GetString("UserEmail");
                var user = await _context.Users.FirstOrDefaultAsync(u => u.EmailId == userEmail);

                if (user == null)
                {
                    return Json(new { success = false, message = "User not found" });
                }

                var student = await _context.StudentDetails
                    .FirstOrDefaultAsync(s => s.UserId == user.UserId);

                var receipt = $"rcpt_{DateTime.Now.Ticks}";
                var order = _paymentService.CreateOrder(amount, student.Document, receipt);

                // Save payment record
                var payment = new Payment
                {
                    UserId = user.UserId,
                    ApplicationId = student.Document,
                    RazorpayOrderId = order["id"].ToString(),
                    Amount = amount,
                    Currency = "INR",
                    Status = "Pending",
                    CreatedAt = DateTime.Now
                };

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    orderId = order["id"],
                    amount = order["amount"],
                    currency = order["currency"]
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Verify Payment
        [HttpPost]
        public async Task<IActionResult> VerifyPayment([FromBody] PaymentVerificationModel model)
        {
            try
            {
                var isValid = _paymentService.VerifyPaymentSignature(
                    model.OrderId,
                    model.PaymentId,
                    model.Signature
                );

                if (isValid)
                {
                    var payment = await _context.Payments
                        .FirstOrDefaultAsync(p => p.RazorpayOrderId == model.OrderId);

                    if (payment != null)
                    {
                        payment.RazorpayPaymentId = model.PaymentId;
                        payment.RazorpaySignature = model.Signature;
                        payment.Status = "Success";
                        payment.UpdatedAt = DateTime.Now;

                        // Update student record
                        var student = await _context.StudentDetails
                            .FirstOrDefaultAsync(s => s.Document == payment.ApplicationId);

                        if (student != null)
                        {
                            student.FeesPaid = "Yes";
                            student.UpdatedAt = DateTime.Now;
                        }

                        await _context.SaveChangesAsync();

                        return Json(new { success = true, message = "Payment successful!" });
                    }
                }

                return Json(new { success = false, message = "Payment verification failed" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Payment verification error");
                return Json(new { success = false, message = "Verification failed" });
            }
        }


        // ============================================
        // EMAIL UPDATE WITH OTP
        // ============================================
        [HttpPost]
        public async Task<IActionResult> SendEmailOtp([FromForm] string newEmail, [FromForm] string applicationId)
        {
            try
            {
                _logger.LogInformation("=== SendEmailOtp Started ===");
                _logger.LogInformation("New Email: {Email}, Application ID: {AppId}", newEmail, applicationId);

                // Validate input
                if (string.IsNullOrWhiteSpace(newEmail) || string.IsNullOrWhiteSpace(applicationId))
                {
                    return Json(new { success = false, message = "Invalid data provided" });
                }

                // Validate email format
                if (!newEmail.Contains("@") || !newEmail.Contains("."))
                {
                    return Json(new { success = false, message = "Please enter a valid email address" });
                }

                // Get current logged-in user from session
                var userEmail = HttpContext.Session.GetString("UserEmail");
                if (string.IsNullOrEmpty(userEmail))
                {
                    return Json(new { success = false, message = "Session expired. Please login again" });
                }

                _logger.LogInformation("Current user email from session: {UserEmail}", userEmail);

                // Get user from database
                var user = await _context.Users.FirstOrDefaultAsync(u => u.EmailId == userEmail);
                if (user == null)
                {
                    _logger.LogWarning("User not found for email: {Email}", userEmail);
                    return Json(new { success = false, message = "User not found" });
                }

                _logger.LogInformation("User found: UserId={UserId}, FullName={Name}", user.UserId, user.FullName);

                // Check if new email is already registered by another student
                var emailExists = await _context.StudentDetails
                    .AnyAsync(s => s.EmailId == newEmail && s.Document != applicationId);

                if (emailExists)
                {
                    _logger.LogWarning("Email {Email} already exists for another student", newEmail);
                    return Json(new { success = false, message = "This email is already registered by another user" });
                }

                // Generate 6-digit OTP
                var random = new Random();
                var otp = random.Next(100000, 999999).ToString();
                _logger.LogInformation("Generated OTP: {Otp}", otp);

                // Save OTP to database
                var otpVerification = new OtpVerification
                {
                    OtpCode = otp,
                    ExpiryTime = DateTime.Now.AddMinutes(10),
                    CreatedAt = DateTime.Now,
                    UserId = user.UserId
                };

                _context.OtpVerifications.Add(otpVerification);
                await _context.SaveChangesAsync();
                _logger.LogInformation("OTP saved to database. OtpId={OtpId}", otpVerification.OtpId);

                // Store new email in session temporarily for verification
                HttpContext.Session.SetString($"PendingEmail_{user.UserId}", newEmail);
                _logger.LogInformation("New email stored in session for UserId={UserId}", user.UserId);

                // Send OTP via Email Service
                bool emailSent = false;
                try
                {
                    emailSent = await _emailService.SendOtpEmailAsync(newEmail, otp, user.FullName);

                    if (emailSent)
                    {
                        _logger.LogInformation("✅ Email sent successfully to {Email}", newEmail);
                    }
                    else
                    {
                        _logger.LogWarning("⚠️ Email sending failed but OTP saved. OTP: {Otp}", otp);
                    }
                }
                catch (Exception emailEx)
                {
                    _logger.LogError(emailEx, "❌ Exception while sending email");
                }

                // Return response
                return Json(new
                {
                    success = true,
                    message = emailSent
                        ? $"OTP has been sent to {newEmail}. Please check your inbox."
                        : $"Email service temporarily unavailable. Test OTP: {otp}"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error in SendEmailOtp");
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }


        [HttpPost]
        public async Task<IActionResult> UpdateEmail([FromForm] string applicationId, [FromForm] string newEmail, [FromForm] string otp)
        {
            try
            {
                _logger.LogInformation("=== UpdateEmail Started ===");
                _logger.LogInformation("Application ID: {AppId}, New Email: {Email}, OTP: {Otp}",
                    applicationId, newEmail, otp);

                // Validate input
                if (string.IsNullOrWhiteSpace(applicationId) ||
                    string.IsNullOrWhiteSpace(newEmail) ||
                    string.IsNullOrWhiteSpace(otp))
                {
                    return Json(new { success = false, message = "All fields are required" });
                }

                // Get current logged-in user
                var userEmail = HttpContext.Session.GetString("UserEmail");
                if (string.IsNullOrEmpty(userEmail))
                {
                    return Json(new { success = false, message = "Session expired. Please login again" });
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.EmailId == userEmail);
                if (user == null)
                {
                    _logger.LogWarning("User not found for email: {Email}", userEmail);
                    return Json(new { success = false, message = "User not found" });
                }

                _logger.LogInformation("User found: UserId={UserId}", user.UserId);

                // Verify email matches what was sent (stored in session)
                var pendingEmail = HttpContext.Session.GetString($"PendingEmail_{user.UserId}");
                if (pendingEmail != newEmail)
                {
                    _logger.LogWarning("Email mismatch. Pending: {Pending}, Received: {Received}",
                        pendingEmail, newEmail);
                    return Json(new { success = false, message = "Email verification mismatch. Please try again." });
                }

                // Find valid OTP for this user
                var otpRecord = await _context.OtpVerifications
                    .Where(o => o.UserId == user.UserId
                             && o.OtpCode == otp
                             && o.ExpiryTime > DateTime.Now)
                    .OrderByDescending(o => o.CreatedAt)
                    .FirstOrDefaultAsync();

                if (otpRecord == null)
                {
                    _logger.LogWarning("Invalid or expired OTP. UserId={UserId}, OTP={Otp}", user.UserId, otp);
                    return Json(new { success = false, message = "Invalid or expired OTP. Please request a new one." });
                }

                _logger.LogInformation("Valid OTP found. OtpId={OtpId}", otpRecord.OtpId);

                // Find student record
                var student = await _context.StudentDetails
                    .FirstOrDefaultAsync(s => s.Document == applicationId);

                if (student == null)
                {
                    _logger.LogWarning("Student not found for ApplicationId: {AppId}", applicationId);
                    return Json(new { success = false, message = "Student record not found" });
                }

                _logger.LogInformation("Student found. UserId={UserId}, Old Email={OldEmail}",
                    student.UserId, student.EmailId);

                // Update student email
                string oldEmail = student.EmailId;
                student.EmailId = newEmail;
                student.UpdatedAt = DateTime.Now;

                // Delete used OTP
                _context.OtpVerifications.Remove(otpRecord);

                // Save changes
                await _context.SaveChangesAsync();

                // Clear session
                HttpContext.Session.Remove($"PendingEmail_{user.UserId}");

                _logger.LogInformation("✅ Email updated successfully!");
                _logger.LogInformation("UserId={UserId}, Old: {Old}, New: {New}",
                    student.UserId, oldEmail, newEmail);

                return Json(new
                {
                    success = true,
                    message = "Email updated successfully! You can now use this email for all communications."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error in UpdateEmail");
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }


        // ============================================
        // MOBILE UPDATE WITH OTP
        // ============================================

        [HttpPost]
        public async Task<IActionResult> SendMobileOtp([FromForm] string newMobile, [FromForm] string applicationId)
        {
            try
            {
                _logger.LogInformation("=== SendMobileOtp Started ===");
                _logger.LogInformation("New Mobile: {Mobile}, Application ID: {AppId}", newMobile, applicationId);

                // Validate input
                if (string.IsNullOrWhiteSpace(newMobile) || string.IsNullOrWhiteSpace(applicationId))
                {
                    return Json(new { success = false, message = "Invalid data provided" });
                }

                // Validate mobile number (10 digits)
                if (newMobile.Length != 10 || !long.TryParse(newMobile, out _))
                {
                    return Json(new { success = false, message = "Please enter a valid 10-digit mobile number" });
                }

                // Get current logged-in user
                var userEmail = HttpContext.Session.GetString("UserEmail");
                if (string.IsNullOrEmpty(userEmail))
                {
                    return Json(new { success = false, message = "Session expired. Please login again" });
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.EmailId == userEmail);
                if (user == null)
                {
                    _logger.LogWarning("User not found for email: {Email}", userEmail);
                    return Json(new { success = false, message = "User not found" });
                }

                _logger.LogInformation("User found: UserId={UserId}", user.UserId);

                // Generate 6-digit OTP
                var random = new Random();
                var otp = random.Next(100000, 999999).ToString();
                _logger.LogInformation("Generated OTP: {Otp}", otp);

                // Save OTP to database
                var otpVerification = new OtpVerification
                {
                    OtpCode = otp,
                    ExpiryTime = DateTime.Now.AddMinutes(10),
                    CreatedAt = DateTime.Now,
                    UserId = user.UserId
                };

                _context.OtpVerifications.Add(otpVerification);
                await _context.SaveChangesAsync();
                _logger.LogInformation("OTP saved to database. OtpId={OtpId}", otpVerification.OtpId);

                // Store new mobile in session
                HttpContext.Session.SetString($"PendingMobile_{user.UserId}", newMobile);

                // TODO: Integrate SMS service here
                // await _smsService.SendOtpAsync(newMobile, otp);

                _logger.LogInformation("✅ Mobile OTP process completed. Test OTP: {Otp}", otp);

                return Json(new
                {
                    success = true,
                    message = $"OTP sent to {newMobile}. For testing: {otp}"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error in SendMobileOtp");
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateMobile([FromForm] string applicationId, [FromForm] string newMobile, [FromForm] string otp)
        {
            try
            {
                _logger.LogInformation("=== UpdateMobile Started ===");
                _logger.LogInformation("Application ID: {AppId}, New Mobile: {Mobile}, OTP: {Otp}",
                    applicationId, newMobile, otp);

                // Validate input
                if (string.IsNullOrWhiteSpace(applicationId) ||
                    string.IsNullOrWhiteSpace(newMobile) ||
                    string.IsNullOrWhiteSpace(otp))
                {
                    return Json(new { success = false, message = "All fields are required" });
                }

                // Get current logged-in user
                var userEmail = HttpContext.Session.GetString("UserEmail");
                if (string.IsNullOrEmpty(userEmail))
                {
                    return Json(new { success = false, message = "Session expired" });
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.EmailId == userEmail);
                if (user == null)
                {
                    return Json(new { success = false, message = "User not found" });
                }

                // Verify mobile matches
                var pendingMobile = HttpContext.Session.GetString($"PendingMobile_{user.UserId}");
                if (pendingMobile != newMobile)
                {
                    return Json(new { success = false, message = "Mobile verification mismatch" });
                }

                // Find valid OTP
                var otpRecord = await _context.OtpVerifications
                    .Where(o => o.UserId == user.UserId
                             && o.OtpCode == otp
                             && o.ExpiryTime > DateTime.Now)
                    .OrderByDescending(o => o.CreatedAt)
                    .FirstOrDefaultAsync();

                if (otpRecord == null)
                {
                    return Json(new { success = false, message = "Invalid or expired OTP" });
                }

                // Update student mobile
                var student = await _context.StudentDetails
                    .FirstOrDefaultAsync(s => s.Document == applicationId);

                if (student == null)
                {
                    return Json(new { success = false, message = "Student record not found" });
                }

                string oldMobile = student.MobileNumber;
                student.MobileNumber = newMobile;
                student.UpdatedAt = DateTime.Now;

                _context.OtpVerifications.Remove(otpRecord);
                await _context.SaveChangesAsync();

                HttpContext.Session.Remove($"PendingMobile_{user.UserId}");

                _logger.LogInformation("✅ Mobile updated: Old={Old}, New={New}", oldMobile, newMobile);

                return Json(new { success = true, message = "Mobile number updated successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error in UpdateMobile");
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }



        [HttpGet]
        public IActionResult Dashboard()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login");
            }

            ViewBag.UserEmail = HttpContext.Session.GetString("UserEmail");
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> SendOtp([FromBody] SendOtpRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.EmailOrMobile))
                    return Json(new { success = false, message = "Email is required" });

                var result = await _authService.ResendOtpAsync(request.EmailOrMobile);
                return Json(new { success = result.Success, message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending OTP");
                return Json(new { success = false, message = "Failed to send OTP" });
            }
        }

        private string GenerateRandomAppId()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 9).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private string GetFormValue(IFormCollection form, string key, string defaultValue = "", int maxLength = 50)
        {
            var value = form[key].ToString()?.Trim();
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }
            return value.Length > maxLength ? value.Substring(0, maxLength) : value;
        }

        [HttpGet]
        public async Task<IActionResult> TestDatabase()
        {
            try
            {
                var canConnect = await _context.Database.CanConnectAsync();
                var userCount = await _context.Users.CountAsync();
                var studentCount = await _context.StudentDetails.CountAsync();

                return Json(new { success = true, canConnect, userCount, studentCount });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }
    }


    public class SendOtpRequest
    {
        public string EmailOrMobile { get; set; } = string.Empty;
    }


}
