using Microsoft.EntityFrameworkCore;
using DoctorateDrive.Models; // where your model classes live

namespace DoctorateDrive.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        public DbSet<StudentDetail> StudentDetails { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<OtpVerification> OtpVerifications { get; set; }
    }
}
