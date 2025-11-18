using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoctorateDrive.Models
{
    [Table("Payments")]
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required, StringLength(50)]
        public string ApplicationId { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string RazorpayOrderId { get; set; } = string.Empty;

        [StringLength(100)]
        public string? RazorpayPaymentId { get; set; }

        [StringLength(100)]
        public string? RazorpaySignature { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required, StringLength(10)]
        public string Currency { get; set; } = "INR";

        [Required, StringLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Success, Failed

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}
