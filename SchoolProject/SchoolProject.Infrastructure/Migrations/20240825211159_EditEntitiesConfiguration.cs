using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolProject.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EditEntitiesConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Instructors_InstructorManager",
                table: "Departments");

            migrationBuilder.DropForeignKey(
                name: "FK_Instructors_Instructors_SupervisorID",
                table: "Instructors");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Instructors_InstructorManager",
                table: "Departments",
                column: "InstructorManager",
                principalTable: "Instructors",
                principalColumn: "InstID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Instructors_Instructors_SupervisorID",
                table: "Instructors",
                column: "SupervisorID",
                principalTable: "Instructors",
                principalColumn: "InstID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Instructors_InstructorManager",
                table: "Departments");

            migrationBuilder.DropForeignKey(
                name: "FK_Instructors_Instructors_SupervisorID",
                table: "Instructors");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Instructors_InstructorManager",
                table: "Departments",
                column: "InstructorManager",
                principalTable: "Instructors",
                principalColumn: "InstID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Instructors_Instructors_SupervisorID",
                table: "Instructors",
                column: "SupervisorID",
                principalTable: "Instructors",
                principalColumn: "InstID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
