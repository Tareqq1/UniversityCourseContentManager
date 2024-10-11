using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS_DEPI.APP.Migrations
{
    /// <inheritdoc />
    public partial class data : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Insert a new User (Teacher)
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Username", "Password", "Role" },
                values: new object[] { 1, "teacher123", "securePassword", "Teacher" }
            );

            // Insert a new Course and associate it with the Teacher (User) using TeacherId
            migrationBuilder.InsertData(
                table: "Courses",
                columns: new[] { "Id", "Title", "Description", "StartDate", "EndDate", "Credits", "TeacherId" },
                values: new object[] { 1, "Introduction to Programming", "This course covers basic programming concepts.", DateTime.Now, DateTime.Now.AddMonths(3), 3, 1 }
            );
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
