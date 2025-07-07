using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddClassTeacherRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassTeachers_Classes_ClassId",
                table: "ClassTeachers");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassTeachers_Teachers_TeacherId",
                table: "ClassTeachers");

            migrationBuilder.DropIndex(
                name: "IX_ClassTeachers_ClassId",
                table: "ClassTeachers");

            migrationBuilder.DropIndex(
                name: "IX_ClassTeachers_TeacherId",
                table: "ClassTeachers");

            migrationBuilder.DropColumn(
                name: "ClassId",
                table: "ClassTeachers");

            migrationBuilder.DropColumn(
                name: "TeacherId",
                table: "ClassTeachers");

            migrationBuilder.CreateIndex(
                name: "IX_ClassTeachers_IdClass",
                table: "ClassTeachers",
                column: "IdClass");

            migrationBuilder.CreateIndex(
                name: "IX_ClassTeachers_IdTeacher",
                table: "ClassTeachers",
                column: "IdTeacher");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassTeachers_Classes_IdClass",
                table: "ClassTeachers",
                column: "IdClass",
                principalTable: "Classes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassTeachers_Teachers_IdTeacher",
                table: "ClassTeachers",
                column: "IdTeacher",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassTeachers_Classes_IdClass",
                table: "ClassTeachers");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassTeachers_Teachers_IdTeacher",
                table: "ClassTeachers");

            migrationBuilder.DropIndex(
                name: "IX_ClassTeachers_IdClass",
                table: "ClassTeachers");

            migrationBuilder.DropIndex(
                name: "IX_ClassTeachers_IdTeacher",
                table: "ClassTeachers");

            migrationBuilder.AddColumn<Guid>(
                name: "ClassId",
                table: "ClassTeachers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TeacherId",
                table: "ClassTeachers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ClassTeachers_ClassId",
                table: "ClassTeachers",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassTeachers_TeacherId",
                table: "ClassTeachers",
                column: "TeacherId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassTeachers_Classes_ClassId",
                table: "ClassTeachers",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassTeachers_Teachers_TeacherId",
                table: "ClassTeachers",
                column: "TeacherId",
                principalTable: "Teachers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
