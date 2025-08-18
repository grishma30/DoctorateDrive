using System;
using System.Collections.Generic;

namespace DoctorateDrive.Models;

public partial class PasswordVerification
{
    public int PasswordId { get; set; }

    public int StudentDetailsId { get; set; }

    public string SentPassword { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;
}
