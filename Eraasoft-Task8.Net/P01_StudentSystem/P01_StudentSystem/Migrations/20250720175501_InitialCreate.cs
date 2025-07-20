using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace P01_StudentSystem.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_HomeWorksSubmission_HomeWorkSubmissionId",
                table: "Students");

            migrationBuilder.DropTable(
                name: "CourseStudent");

            migrationBuilder.DropIndex(
                name: "IX_Students_HomeWorkSubmissionId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "HomeWorkSubmissionId",
                table: "Students");

            migrationBuilder.RenameColumn(
                name: "HomeWorkSubmissionId",
                table: "HomeWorksSubmission",
                newName: "HomeworkId");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Students",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "Birthday",
                table: "Students",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<int>(
                name: "ResourceType",
                table: "Resources",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Resources",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "ContentType",
                table: "HomeWorksSubmission",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Courses",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Courses",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "StudentCourses",
                columns: table => new
                {
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentCourses", x => new { x.StudentId, x.CourseId });
                    table.ForeignKey(
                        name: "FK_StudentCourses_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentCourses_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "StudentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Courses",
                columns: new[] { "CourseId", "Description", "EndDate", "Name", "Price", "StartDate" },
                values: new object[,]
                {
                    { 1, "Learn the basics of C# programming language including syntax, OOP concepts, and basic data structures.", new DateOnly(2024, 5, 30), "C# Programming Fundamentals", 1500.00m, new DateOnly(2024, 3, 1) },
                    { 2, "Build modern web applications using ASP.NET Core, Entity Framework, and MVC architecture.", new DateOnly(2024, 7, 31), "ASP.NET Core Web Development", 2500.00m, new DateOnly(2024, 4, 1) },
                    { 3, "Master database design principles, SQL queries, and database optimization techniques.", new DateOnly(2024, 6, 15), "Database Design and SQL", 1800.00m, new DateOnly(2024, 3, 15) },
                    { 4, null, new DateOnly(2024, 7, 1), "JavaScript for Beginners", 1200.00m, new DateOnly(2024, 5, 1) },
                    { 5, "Build interactive user interfaces using React, hooks, and modern JavaScript features.", new DateOnly(2024, 8, 31), "React Frontend Development", 2200.00m, new DateOnly(2024, 6, 1) }
                });

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "StudentId", "Birthday", "Name", "PhoneNumber", "RegisteredOn" },
                values: new object[,]
                {
                    { 1, new DateOnly(2000, 5, 15), "Ahmed Mohamed Ali", "0123456789", new DateTime(2024, 1, 15, 10, 30, 0, 0, DateTimeKind.Unspecified) },
                    { 2, new DateOnly(1999, 8, 22), "Sara Hassan Ibrahim", "0198765432", new DateTime(2024, 1, 20, 14, 15, 0, 0, DateTimeKind.Unspecified) },
                    { 3, null, "Omar Khaled Mahmoud", null, new DateTime(2024, 2, 1, 9, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, new DateOnly(2001, 12, 3), "Fatma Adel Mostafa", "0111222333", new DateTime(2024, 2, 10, 16, 45, 0, 0, DateTimeKind.Unspecified) },
                    { 5, new DateOnly(1998, 3, 10), "Youssef Tarek Ahmed", "0155667788", new DateTime(2024, 2, 15, 11, 20, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "HomeWorksSubmission",
                columns: new[] { "HomeworkId", "Content", "ContentType", "CourseId", "StudentId", "SubmissionTime" },
                values: new object[,]
                {
                    { 1, "https://example.com/ahmed-csharp-assignment1.zip", 2, 1, 1, new DateTime(2024, 3, 15, 23, 45, 0, 0, DateTimeKind.Unspecified) },
                    { 2, "https://example.com/fatma-csharp-assignment1.pdf", 1, 1, 4, new DateTime(2024, 3, 14, 20, 30, 0, 0, DateTimeKind.Unspecified) },
                    { 3, "https://example.com/sara-aspnet-project.zip", 2, 2, 2, new DateTime(2024, 4, 20, 18, 15, 0, 0, DateTimeKind.Unspecified) },
                    { 4, "https://example.com/youssef-web-app.zip", 2, 2, 5, new DateTime(2024, 4, 22, 22, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, "https://example.com/ahmed-sql-queries.pdf", 1, 3, 1, new DateTime(2024, 4, 1, 16, 30, 0, 0, DateTimeKind.Unspecified) },
                    { 6, "https://example.com/sara-database-design.zip", 2, 3, 2, new DateTime(2024, 4, 5, 14, 45, 0, 0, DateTimeKind.Unspecified) },
                    { 7, "https://example.com/omar-js-calculator", 0, 4, 3, new DateTime(2024, 5, 15, 19, 20, 0, 0, DateTimeKind.Unspecified) },
                    { 8, "https://example.com/omar-react-todo-app.zip", 2, 5, 3, new DateTime(2024, 6, 20, 21, 10, 0, 0, DateTimeKind.Unspecified) },
                    { 9, "https://example.com/youssef-react-portfolio.zip", 2, 5, 5, new DateTime(2024, 6, 25, 23, 30, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Resources",
                columns: new[] { "ResourceId", "CourseId", "Name", "ResourceType", "Url" },
                values: new object[,]
                {
                    { 1, 1, "C# Basics Video Tutorial", 0, "https://example.com/csharp-basics-video" },
                    { 2, 1, "C# Reference Guide", 2, "https://example.com/csharp-reference.pdf" },
                    { 3, 1, "OOP Concepts Presentation", 1, "https://example.com/oop-presentation.pptx" },
                    { 4, 2, "ASP.NET Core Setup Guide", 2, "https://example.com/aspnet-setup.pdf" },
                    { 5, 2, "MVC Architecture Video", 0, "https://example.com/mvc-architecture-video" },
                    { 6, 3, "SQL Query Examples", 3, "https://example.com/sql-examples" },
                    { 7, 3, "Database Design Principles", 1, "https://example.com/db-design.pptx" },
                    { 8, 4, "JavaScript Fundamentals Video", 0, "https://example.com/js-fundamentals" },
                    { 9, 5, "React Components Guide", 2, "https://example.com/react-components.pdf" },
                    { 10, 5, "React Hooks Tutorial", 0, "https://example.com/react-hooks-video" }
                });

            migrationBuilder.InsertData(
                table: "StudentCourses",
                columns: new[] { "CourseId", "StudentId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 3, 1 },
                    { 2, 2 },
                    { 3, 2 },
                    { 4, 3 },
                    { 5, 3 },
                    { 1, 4 },
                    { 2, 4 },
                    { 1, 5 },
                    { 2, 5 },
                    { 3, 5 },
                    { 5, 5 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_HomeWorksSubmission_StudentId",
                table: "HomeWorksSubmission",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentCourses_CourseId",
                table: "StudentCourses",
                column: "CourseId");

            migrationBuilder.AddForeignKey(
                name: "FK_HomeWorksSubmission_Students_StudentId",
                table: "HomeWorksSubmission",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "StudentId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HomeWorksSubmission_Students_StudentId",
                table: "HomeWorksSubmission");

            migrationBuilder.DropTable(
                name: "StudentCourses");

            migrationBuilder.DropIndex(
                name: "IX_HomeWorksSubmission_StudentId",
                table: "HomeWorksSubmission");

            migrationBuilder.DeleteData(
                table: "HomeWorksSubmission",
                keyColumn: "HomeworkId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "HomeWorksSubmission",
                keyColumn: "HomeworkId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "HomeWorksSubmission",
                keyColumn: "HomeworkId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "HomeWorksSubmission",
                keyColumn: "HomeworkId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "HomeWorksSubmission",
                keyColumn: "HomeworkId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "HomeWorksSubmission",
                keyColumn: "HomeworkId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "HomeWorksSubmission",
                keyColumn: "HomeworkId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "HomeWorksSubmission",
                keyColumn: "HomeworkId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "HomeWorksSubmission",
                keyColumn: "HomeworkId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Resources",
                keyColumn: "ResourceId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Resources",
                keyColumn: "ResourceId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Resources",
                keyColumn: "ResourceId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Resources",
                keyColumn: "ResourceId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Resources",
                keyColumn: "ResourceId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Resources",
                keyColumn: "ResourceId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Resources",
                keyColumn: "ResourceId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Resources",
                keyColumn: "ResourceId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Resources",
                keyColumn: "ResourceId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Resources",
                keyColumn: "ResourceId",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "CourseId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "CourseId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "CourseId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "CourseId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "CourseId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 5);

            migrationBuilder.RenameColumn(
                name: "HomeworkId",
                table: "HomeWorksSubmission",
                newName: "HomeWorkSubmissionId");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Students",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "Birthday",
                table: "Students",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HomeWorkSubmissionId",
                table: "Students",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ResourceType",
                table: "Resources",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Resources",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "ContentType",
                table: "HomeWorksSubmission",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Price",
                table: "Courses",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "CourseStudent",
                columns: table => new
                {
                    CoursesCourseId = table.Column<int>(type: "int", nullable: false),
                    StudentsStudentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseStudent", x => new { x.CoursesCourseId, x.StudentsStudentId });
                    table.ForeignKey(
                        name: "FK_CourseStudent_Courses_CoursesCourseId",
                        column: x => x.CoursesCourseId,
                        principalTable: "Courses",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseStudent_Students_StudentsStudentId",
                        column: x => x.StudentsStudentId,
                        principalTable: "Students",
                        principalColumn: "StudentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Students_HomeWorkSubmissionId",
                table: "Students",
                column: "HomeWorkSubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseStudent_StudentsStudentId",
                table: "CourseStudent",
                column: "StudentsStudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_HomeWorksSubmission_HomeWorkSubmissionId",
                table: "Students",
                column: "HomeWorkSubmissionId",
                principalTable: "HomeWorksSubmission",
                principalColumn: "HomeWorkSubmissionId");
        }
    }
}
