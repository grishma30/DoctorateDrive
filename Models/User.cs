using System;
using System.Collections.Generic;

namespace DoctorateDrive.Models;

public partial class User
{
    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string EmailId { get; set; } = null!;

    public string MobileNumber { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual ICollection<OtpVerification> OtpVerifications { get; set; } = new List<OtpVerification>();

<<<<<<< HEAD
        [NotMapped]
        public string PasswordHash { get; set; } = string.Empty;

        public string? JWTtoken { get; set; }
    }
=======
    public virtual ICollection<StudentDetail> StudentDetails { get; set; } = new List<StudentDetail>();
>>>>>>> 90b09ce49e01b7f3a241324b844c652d86626344
}
