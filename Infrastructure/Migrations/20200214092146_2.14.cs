using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Migrations
{
    public partial class _214 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MeetingStats",
                table: "Meeting");

            migrationBuilder.AddColumn<int>(
                name: "MeetingState",
                table: "Meeting",
                maxLength: 20,
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MeetingState",
                table: "Meeting");

            migrationBuilder.AddColumn<int>(
                name: "MeetingStats",
                table: "Meeting",
                type: "int",
                maxLength: 20,
                nullable: false,
                defaultValue: 0);
        }
    }
}
