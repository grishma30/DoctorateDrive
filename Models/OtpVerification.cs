using System;
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
}
