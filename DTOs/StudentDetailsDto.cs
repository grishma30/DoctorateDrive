using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;



namespace DoctorateDrive.DTOs
{
    public class StudentDetailsDto
    {
        // ============================================
        // ACADEMIC DETAILS
        // ============================================
        [Required(ErrorMessage = "CGPA gained is required")]
        [Range(0, 10, ErrorMessage = "CGPA must be between 0 and 10")]
        public decimal CgpaInput { get; set; }

        [Required(ErrorMessage = "Total CGPA is required")]
        [Range(0, 10, ErrorMessage = "Total CGPA must be between 0 and 10")]
        public decimal TotalCgpa { get; set; }

        [Required(ErrorMessage = "Equivalent percentage is required")]
        [Range(0, 100, ErrorMessage = "Percentage must be between 0 and 100")]
        public decimal EquivalentPercentage { get; set; }

        // Graduate Qualification
        public string? GraduateSelect { get; set; }
        public IFormFile? GraduateCertificate { get; set; }

        // Post Graduate Qualification
        public string? PostGraduateSelect { get; set; }
        public IFormFile? PostGraduateCertificate { get; set; }

        // GATE Qualification
        public string? GateSelect { get; set; }
        public IFormFile? GateCertificate { get; set; }

        // Conversion Certificate (for CGPA to Percentage)
        public IFormFile? ConversionCertificate { get; set; }

        public string? PreferredInstitute { get; set; }
        public string? PreferredSpecialization { get; set; }

        // ============================================
        // PERSONAL DETAILS
        // ============================================
        [Required(ErrorMessage = "First name is required")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
        public string FirstName { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Middle name cannot exceed 50 characters")]
        public string? MiddleName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mobile number is required")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Mobile number must be 10 digits")]
        public string MobileNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "WhatsApp number is required")]
        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "WhatsApp number must be 10 digits")]
        public string WhatsappNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date of birth is required")]
        public DateTime Dob { get; set; }

        [Required(ErrorMessage = "Caste category is required")]
        [StringLength(50)]
        public string CasteCategory { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nationality is required")]
        [StringLength(50)]
        public string Nationality { get; set; } = string.Empty;

        [Required(ErrorMessage = "Gender is required")]
        [StringLength(50)]
        public string Gender { get; set; } = string.Empty;

        // ============================================
        // GUARDIAN DETAILS
        // ============================================
        [Required(ErrorMessage = "Guardian name is required")]
        [StringLength(100, ErrorMessage = "Guardian name cannot exceed 100 characters")]
        public string GuardianName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Relation with guardian is required")]
        [StringLength(50)]
        public string RelationWithApplicant { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Invalid guardian email format")]
        [StringLength(100)]
        public string? GuardianEmail { get; set; }

        [RegularExpression(@"^[0-9]{10}$", ErrorMessage = "Guardian mobile must be 10 digits")]
        public string? GuardianMobile { get; set; }

        // ============================================
        // ADDRESS DETAILS
        // ============================================
        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Country is required")]
        [StringLength(50)]
        public string Country { get; set; } = string.Empty;

        [Required(ErrorMessage = "State is required")]
        [StringLength(50)]
        public string State { get; set; } = string.Empty;

        [Required(ErrorMessage = "District is required")]
        [StringLength(50)]
        public string District { get; set; } = string.Empty;

        [Required(ErrorMessage = "City is required")]
        [StringLength(50)]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "PIN code is required")]
        [RegularExpression(@"^[0-9]{6}$", ErrorMessage = "PIN code must be 6 digits")]
        public string Pin { get; set; } = string.Empty;
    }
}
