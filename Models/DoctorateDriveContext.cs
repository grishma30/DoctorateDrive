using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DoctorateDrive.Models;

public partial class DoctorateDriveContext : DbContext
{
    public DoctorateDriveContext()
    {
    }

    public DoctorateDriveContext(DbContextOptions<DoctorateDriveContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Document> Documents { get; set; }

    public virtual DbSet<OtpVerification> OtpVerifications { get; set; }

    public virtual DbSet<PasswordVerification> PasswordVerifications { get; set; }

    public virtual DbSet<StudentDetail> StudentDetails { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-1NKAFK4\\SQLEXPRESS;Database=doctorateDrive;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;MultipleActiveResultSets=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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

        modelBuilder.Entity<OtpVerification>(entity =>
        {
            entity.HasKey(e => e.OtpId);

            entity.ToTable("OtpVerification");

            entity.Property(e => e.OtpId).ValueGeneratedOnAdd();
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ExpiryTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.OtpCode).HasMaxLength(10);
            entity.Property(e => e.StudentId).HasColumnName("StudentID");

            entity.HasOne(d => d.Otp).WithOne(p => p.OtpVerification)
                .HasForeignKey<OtpVerification>(d => d.OtpId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_StudentDetails_OtpVerification");
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

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EmailId).HasMaxLength(100);
            entity.Property(e => e.FullName).HasMaxLength(50);
            entity.Property(e => e.MobileNumber).HasMaxLength(15);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Role).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
