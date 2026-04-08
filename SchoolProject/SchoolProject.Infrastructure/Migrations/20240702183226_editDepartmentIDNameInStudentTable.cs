using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolProject.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class editDepartmentIDNameInStudentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_Departments_DepartmentDeptID",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_DepartmentDeptID",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "DepartmentDeptID",
                table: "Students");

            migrationBuilder.RenameColumn(
                name: "DeptID",
                table: "Students",
                newName: "DepartmentID");

            migrationBuilder.CreateIndex(
                name: "IX_Students_DepartmentID",
                table: "Students",
                column: "DepartmentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Departments_DepartmentID",
                table: "Students",
                column: "DepartmentID",
                principalTable: "Departments",
                principalColumn: "DeptID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_Departments_DepartmentID",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_DepartmentID",
                table: "Students");

            migrationBuilder.RenameColumn(
                name: "DepartmentID",
                table: "Students",
                newName: "DeptID");

            migrationBuilder.AddColumn<int>(
                name: "DepartmentDeptID",
                table: "Students",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Students_DepartmentDeptID",
                table: "Students",
                column: "DepartmentDeptID");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Departments_DepartmentDeptID",
                table: "Students",
                column: "DepartmentDeptID",
                principalTable: "Departments",
                principalColumn: "DeptID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
