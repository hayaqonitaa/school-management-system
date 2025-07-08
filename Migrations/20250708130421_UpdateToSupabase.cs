using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class UpdateToSupabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Enrollment_Student_ClassTeacher",
                table: "Enrollments");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollment_OneStudentOneClass",
                table: "Enrollments",
                column: "IdStudent",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Enrollment_OneStudentOneClass",
                table: "Enrollments");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollment_Student_ClassTeacher",
                table: "Enrollments",
                columns: new[] { "IdStudent", "IdClassTeacher" },
                unique: true);
        }
    }
}
