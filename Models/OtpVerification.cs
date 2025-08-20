using System;
using System.Collections.Generic;

namespace DoctorateDrive.Models;

public partial class OtpVerification
{
    public int OtpId { get; set; }

    public int StudentId { get; set; }

    public string OtpCode { get; set; } = null!;

    public DateTime ExpiryTime { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual StudentDetail Otp { get; set; } = null!;
}
