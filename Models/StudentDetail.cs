using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoctorateDrive.Models
{
    [Table("StudentDetails")]
    public class StudentDetail
    {
        [Key]

        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required, StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [StringLength(50)]
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

        [StringLength(50)]
        public string? PreferredInstitute { get; set; }

        [StringLength(100)]
        public string? PreferredSpecialization { get; set; }


        // NEW FIELDS - Added
        public string? GraduateQualification { get; set; }
        public string? GraduateCertificatePath { get; set; }
        public string? PostGraduateQualification { get; set; }
        public string? PostGraduateCertificatePath { get; set; }
        public string? GateCertificatePath { get; set; }

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

        [StringLength(100)]
        public string GuardianEmail { get; set; } = string.Empty;

        [StringLength(15)]
        public string GuardianMobileNumber { get; set; } = string.Empty;

        [StringLength(50)]
        public string Document { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        [NotMapped]
        public string Pin
        {
            get => PIN;
            set => PIN = value;
        }

        [NotMapped]
        public bool Gatequalified
        {
            get => GATEQualified;
            set => GATEQualified = value;
        }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}
