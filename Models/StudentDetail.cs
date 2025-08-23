using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoctorateDrive.Models
{
    [Table("StudentDetails")]
    public class StudentDetail
    {
        [Key]
        public int StudentId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required, StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string MiddleName { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public DateTime DOB { get; set; }

        [Required, StringLength(50)]
        public string Gender { get; set; } = string.Empty;

        [Required, StringLength(15)]
        public string MobileNumber { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string CasteCategory { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string Nationality { get; set; } = string.Empty;

        [Column(TypeName = "decimal(4, 2)")]
        public decimal CGPAGained { get; set; }

        [Column(TypeName = "decimal(4, 2)")]
        public decimal CGPATotal { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        public decimal Percentage { get; set; }

        public bool GATEQualified { get; set; }

        [Required, StringLength(10)]
        public string FeesPaid { get; set; } = string.Empty;

        [Required, StringLength(15)]
        public string WhatsappNumber { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string EmailId { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string Country { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string State { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string City { get; set; } = string.Empty;

        [Required, StringLength(10)]
        public string PIN { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string District { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string GuardianName { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string RelationWithGuardian { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string GuardianEmail { get; set; } = string.Empty;

        [Required, StringLength(15)]
        public string GuardianMobileNumber { get; set; } = string.Empty;

        // Additional properties based on errors, added with correct casing
        public string Document { get; set; } = string.Empty;
        public string Pin { get; set; } = string.Empty;
        public bool Gatequalified { get; set; }

        // Navigation property
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
    }
}
