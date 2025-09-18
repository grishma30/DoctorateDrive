using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoctorateDrive.Migrations.DoctorateDrive
{
    /// <inheritdoc />
    public partial class AddStudentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OtpVerification_Users_UserId",
                table: "OtpVerification");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationId",
                table: "StudentDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "OtpVerification",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Otp_User",
                table: "OtpVerification",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Otp_User",
                table: "OtpVerification");

            migrationBuilder.DropColumn(
                name: "ApplicationId",
                table: "StudentDetails");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "OtpVerification",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OtpVerification_Users_UserId",
                table: "OtpVerification",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
