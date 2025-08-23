using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoctorateDrive.Models
{
    [Table("OtpVerification")]
    public class OtpVerification
    {
        [Key]
        public int OtpId { get; set; }

        [Required]
        public int StudentID { get; set; }

        [Required]
        [StringLength(10)]
        public string OtpCode { get; set; } = string.Empty;

        [Required]
        public DateTime ExpiryTime { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        // Remove any [NotMapped] properties or references to "Otp", "StudentId", etc.
        // Only keep properties that exactly match your database columns
    }
}
