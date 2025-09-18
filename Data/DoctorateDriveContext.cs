using DoctorateDrive.Models;
using DoctorateDrive.Data;
using Microsoft.EntityFrameworkCore;

namespace DoctorateDrive.Data
{
    public partial class DoctorateDriveContext : DbContext
    {
        public DoctorateDriveContext(DbContextOptions<DoctorateDriveContext> options) : base(options)
        {
        }

        // Only include DbSets for entities that exist in your database
        public DbSet<User> Users { get; set; }
        public DbSet<OtpVerification> OtpVerifications { get; set; }
        public virtual DbSet<Document> Documents { get; set; }
        public virtual DbSet<PasswordVerification> PasswordVerifications { get; set; }

        public virtual DbSet<StudentDetail> StudentDetails { get; set; }

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

            modelBuilder.Entity<Document>(entity =>
            {
                entity.Property(e => e.DocumentId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("DocumentID");
                entity.Property(e => e.DocumentType).HasMaxLength(50);
                entity.Property(e => e.FilePath).HasMaxLength(255);
                entity.Property(e => e.StudentId).HasColumnName("StudentID");
                entity.Property(e => e.UploadedDate)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.DocumentNavigation).WithOne(p => p.Document)
                    .HasForeignKey<Document>(d => d.DocumentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StudentDetails_Documents");
            });

            modelBuilder.Entity<PasswordVerification>(entity =>
            {
                entity.HasKey(e => e.PasswordId);

                entity.ToTable("PasswordVerification");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .HasColumnName("email");
                entity.Property(e => e.Password).HasMaxLength(50);
                entity.Property(e => e.SentPassword).HasMaxLength(50);
            });

            modelBuilder.Entity<StudentDetail>(entity =>
            {
                entity.HasKey(e => e.StudentId);

                entity.HasIndex(e => e.EmailId, "UQ_Users_Email").IsUnique();

                entity.Property(e => e.CasteCategory).HasMaxLength(50);
                entity.Property(e => e.Cgpagained)
                    .HasColumnType("decimal(4, 2)")
                    .HasColumnName("CGPAGained");
                entity.Property(e => e.Cgpatotal)
                    .HasColumnType("decimal(4, 2)")
                    .HasColumnName("CGPATotal");
                entity.Property(e => e.City).HasMaxLength(50);
                entity.Property(e => e.Country).HasMaxLength(50);
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.District).HasMaxLength(50);
                entity.Property(e => e.Dob).HasColumnName("DOB");
                entity.Property(e => e.EmailId).HasMaxLength(100);
                entity.Property(e => e.FeesPaid)
                    .HasMaxLength(10)
                    .IsFixedLength();
                entity.Property(e => e.FirstName).HasMaxLength(50);
                entity.Property(e => e.Gatequalified).HasColumnName("GATEQualified");
                entity.Property(e => e.Gender).HasMaxLength(50);
                entity.Property(e => e.GuardianEmail).HasMaxLength(100);
                entity.Property(e => e.GuardianMobileNumber).HasMaxLength(15);
                entity.Property(e => e.GuardianName).HasMaxLength(100);
                entity.Property(e => e.LastName).HasMaxLength(50);
                entity.Property(e => e.MiddleName).HasMaxLength(50);
                entity.Property(e => e.MobileNumber).HasMaxLength(15);
                entity.Property(e => e.Nationality).HasMaxLength(50);
                entity.Property(e => e.Percentage).HasColumnType("decimal(5, 2)");
                entity.Property(e => e.Pin)
                    .HasMaxLength(10)
                    .HasColumnName("PIN");
                entity.Property(e => e.RelationWithGuardian).HasMaxLength(50);
                entity.Property(e => e.State).HasMaxLength(50);
                entity.Property(e => e.UpdatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.WhatsappNumber).HasMaxLength(15);

                entity.HasOne(d => d.User).WithMany(p => p.StudentDetails)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Users_StudentDetails");
            });


        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    }
}
