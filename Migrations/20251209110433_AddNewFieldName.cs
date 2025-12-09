using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoctorateDrive.Migrations
{
    /// <inheritdoc />
    public partial class AddNewFieldName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    DocumentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentID = table.Column<int>(type: "int", nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UploadedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.DocumentID);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    PaymentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ApplicationId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RazorpayOrderId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RazorpayPaymentId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    RazorpaySignature = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.PaymentId);
                    table.ForeignKey(
                        name: "FK_Payments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentDetails",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MiddleName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DOB = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MobileNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    CasteCategory = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nationality = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CGPAGained = table.Column<decimal>(type: "decimal(4,2)", nullable: false),
                    CGPATotal = table.Column<decimal>(type: "decimal(4,2)", nullable: false),
                    Percentage = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    GATEQualified = table.Column<bool>(type: "bit", nullable: false),
                    PreferredInstitute = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PreferredSpecialization = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    GraduateQualification = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GraduateCertificatePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostGraduateQualification = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostGraduateCertificatePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GateCertificatePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EquivalentCgpaCertificatePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FeesPaid = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    WhatsappNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    EmailId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    State = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    City = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PIN = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    District = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GuardianName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RelationWithGuardian = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GuardianEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    GuardianMobileNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Document = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentDetails", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_StudentDetails_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_UserId",
                table: "Payments",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "StudentDetails");
        }
    }
}
