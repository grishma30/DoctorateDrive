namespace DoctorateDrive.DTOs
{
    public class StudentDetailDto
    {
        public int UserId { get; set; }   // from logged-in user

        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public DateOnly Dob { get; set; }
        public string Gender { get; set; }
        public string MobileNumber { get; set; }
        public string CasteCategory { get; set; }
        public string Nationality { get; set; }
        public decimal Cgpagained { get; set; }
        public decimal Cgpatotal { get; set; }
        public decimal Percentage { get; set; }
        public bool Gatequalified { get; set; }
        public string FeesPaid { get; set; }
        public string WhatsappNumber { get; set; }
        public string EmailId { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Pin { get; set; }
        public string District { get; set; }
        public string GuardianName { get; set; }
        public string RelationWithGuardian { get; set; }
        public string GuardianEmail { get; set; }
        public string GuardianMobileNumber { get; set; }
    }
}
