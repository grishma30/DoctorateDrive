using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoctorateDrive.Models
{
    [Table("PasswordVerification")]
    public class PasswordVerification
    {
        [Key]
        public int PasswordId { get; set; }

        [Required]
        public int StudentDetailsId { get; set; }

        [Required]
        [StringLength(50)]
        public string SentPassword { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Column("email")]  // Lowercase column name as per your database
        public string Email { get; set; } = string.Empty;
    }
}
