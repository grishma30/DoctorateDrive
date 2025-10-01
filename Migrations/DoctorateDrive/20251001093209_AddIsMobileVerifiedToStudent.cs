using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoctorateDrive.Migrations.DoctorateDrive
{
    /// <inheritdoc />
    public partial class AddIsMobileVerifiedToStudent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentDetails_Documents",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_OtpVerifications_Users_UserId",
                table: "OtpVerifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OtpVerifications",
                table: "OtpVerifications");

            migrationBuilder.RenameTable(
                name: "OtpVerifications",
                newName: "OtpVerification");

            migrationBuilder.RenameIndex(
                name: "IX_OtpVerifications_UserId",
                table: "OtpVerification",
                newName: "IX_OtpVerification_UserId");

            migrationBuilder.AddColumn<string>(
                name: "Jwttoken",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DocumentsDocumentId",
                table: "StudentDetails",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsMobileVerified",
                table: "StudentDetails",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "DocumentID",
                table: "Documents",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OtpVerification",
                table: "OtpVerification",
                column: "OtpId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentDetails_DocumentsDocumentId",
                table: "StudentDetails",
                column: "DocumentsDocumentId");

            migrationBuilder.AddForeignKey(
                name: "FK_OtpVerification_Users_UserId",
                table: "OtpVerification",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentDetails_Documents_DocumentsDocumentId",
                table: "StudentDetails",
                column: "DocumentsDocumentId",
                principalTable: "Documents",
                principalColumn: "DocumentID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OtpVerification_Users_UserId",
                table: "OtpVerification");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentDetails_Documents_DocumentsDocumentId",
                table: "StudentDetails");

            migrationBuilder.DropIndex(
                name: "IX_StudentDetails_DocumentsDocumentId",
                table: "StudentDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OtpVerification",
                table: "OtpVerification");

            migrationBuilder.DropColumn(
                name: "Jwttoken",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DocumentsDocumentId",
                table: "StudentDetails");

            migrationBuilder.DropColumn(
                name: "IsMobileVerified",
                table: "StudentDetails");

            migrationBuilder.RenameTable(
                name: "OtpVerification",
                newName: "OtpVerifications");

            migrationBuilder.RenameIndex(
                name: "IX_OtpVerification_UserId",
                table: "OtpVerifications",
                newName: "IX_OtpVerifications_UserId");

            migrationBuilder.AlterColumn<int>(
                name: "DocumentID",
                table: "Documents",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OtpVerifications",
                table: "OtpVerifications",
                column: "OtpId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentDetails_Documents",
                table: "Documents",
                column: "DocumentID",
                principalTable: "StudentDetails",
                principalColumn: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_OtpVerifications_Users_UserId",
                table: "OtpVerifications",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }
    }
}
