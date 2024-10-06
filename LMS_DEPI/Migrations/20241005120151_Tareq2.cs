using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS_DEPI.APP.Migrations
{
    /// <inheritdoc />
    public partial class Tareq2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "Lessons",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "Lessons");
        }
    }
}
