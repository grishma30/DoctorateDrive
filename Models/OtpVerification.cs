<<<<<<< HEAD
﻿using DoctorateDrive.Models;
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
=======
﻿using System;
using System.Collections.Generic;

namespace DoctorateDrive.Models;

public partial class OtpVerification
{
    public int OtpId { get; set; }

    public string OtpCode { get; set; } = null!;

    public DateTime ExpiryTime { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? UserId { get; set; }

    public virtual User? User { get; set; }
>>>>>>> 90b09ce49e01b7f3a241324b844c652d86626344
}
