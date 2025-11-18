using DoctorateDrive.Models;
using Microsoft.EntityFrameworkCore;

namespace DoctorateDrive.Data
{
    public class DoctorateDriveContext : DbContext
    {
        public DoctorateDriveContext(DbContextOptions<DoctorateDriveContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<OtpVerification> OtpVerifications { get; set; }
        public DbSet<StudentDetail> StudentDetails { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Payment> Payments { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.UserId);
                entity.Property(u => u.CreatedAt).HasDefaultValueSql("getdate()");
                entity.Property(u => u.UpdatedAt).HasDefaultValueSql("getdate()");
            });

            // OtpVerification configuration
            modelBuilder.Entity<OtpVerification>(entity =>
            {
                entity.HasKey(o => o.OtpId);
                entity.Property(o => o.CreatedAt).HasDefaultValueSql("getdate()");
                entity.Property(o => o.ExpiryTime).HasDefaultValueSql("getdate()");
            });

            // Payment entity configuration
            //modelBuilder.Entity<Payment>(entity =>
            //{
            //    entity.HasKey(e => e.PaymentId);
            //    entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            //    entity.Property(e => e.PaymentDate).HasDefaultValueSql("getdate()");
            //});

            // StudentDetail configuration - UserId is PRIMARY KEY
            modelBuilder.Entity<StudentDetail>(entity =>
            {
                entity.HasKey(s => s.UserId);  // ✅ UserId is the primary key
                entity.HasOne(s => s.User)
                      .WithOne()
                      .HasForeignKey<StudentDetail>(s => s.UserId);
            });
        }
    }
}
