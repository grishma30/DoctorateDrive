using DoctorateDrive.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("OtpVerification")]
public class OtpVerification
{
    [Key]
    public int OtpId { get; set; }

    [Required]
    public int UserId { get; set; }  // Changed from StudentID

    [Required]
    [StringLength(10)]
    public string OtpCode { get; set; } = string.Empty;

    [Required]
    public DateTime ExpiryTime { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("UserId")]
    public virtual User User { get; set; }
}
