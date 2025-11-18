namespace DoctorateDrive.DTOs
{
    public class StudentDetailViewDto
    {
        public string ApplicationId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }
}
