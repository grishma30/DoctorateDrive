using System;
using System.Collections.Generic;

namespace DoctorateDrive.Models;

public partial class StudentDetail
{
    public int StudentId { get; set; }

    public int UserId { get; set; }

    public string FirstName { get; set; } = null!;

    public string MiddleName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateOnly Dob { get; set; }

    public string Gender { get; set; } = null!;

    public string MobileNumber { get; set; } = null!;

    public string CasteCategory { get; set; } = null!;

    public string Nationality { get; set; } = null!;

    public decimal Cgpagained { get; set; }

    public decimal Cgpatotal { get; set; }

    public decimal Percentage { get; set; }

    public bool Gatequalified { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public string FeesPaid { get; set; } = null!;

    public string WhatsappNumber { get; set; } = null!;

    public string EmailId { get; set; } = null!;

    public string Country { get; set; } = null!;

    public string State { get; set; } = null!;

    public string City { get; set; } = null!;

    public string Pin { get; set; } = null!;

    public string District { get; set; } = null!;

    public string GuardianName { get; set; } = null!;

    public string RelationWithGuardian { get; set; } = null!;

    public string GuardianEmail { get; set; } = null!;

    public string GuardianMobileNumber { get; set; } = null!;

    public virtual Document? Document { get; set; }

    public virtual OtpVerification? OtpVerification { get; set; }

    public virtual User User { get; set; } = null!;
}
