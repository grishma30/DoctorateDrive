using DoctorateDrive.Models;
using Microsoft.EntityFrameworkCore;

namespace DoctorateDrive.Data
{
    public class DoctorateDriveContext : DbContext
    {
        public DoctorateDriveContext(DbContextOptions<DoctorateDriveContext> options) : base(options)
        {
        }

        // Only include DbSets for entities that exist in your database
        public DbSet<User> Users { get; set; }
        public DbSet<OtpVerification> OtpVerifications { get; set; }

        // Remove any references to StudentDetail, Document, etc. if they don't exist

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User entity configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.UserId);
                entity.Property(u => u.CreatedAt).HasDefaultValueSql("getdate()");
                entity.Property(u => u.UpdatedAt).HasDefaultValueSql("getdate()");
            });

            // OtpVerification entity configuration
            modelBuilder.Entity<OtpVerification>(entity =>
            {
                entity.HasKey(o => o.OtpId);
                entity.Property(o => o.CreatedAt).HasDefaultValueSql("getdate()");
                entity.Property(o => o.ExpiryTime).HasDefaultValueSql("getdate()");
            });

            // Remove ALL configurations for non-existent entities
            // like StudentDetail, Document, PasswordVerification, etc.
        }
    }
}
