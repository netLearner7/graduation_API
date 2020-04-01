using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Migrations
{
    public partial class _1132 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Stats",
                table: "User_Meeting");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Meeting");

            migrationBuilder.AddColumn<int>(
                name: "User_MeetingStats",
                table: "User_Meeting",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MeetingStats",
                table: "Meeting",
                maxLength: 20,
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "User_MeetingStats",
                table: "User_Meeting");

            migrationBuilder.DropColumn(
                name: "MeetingStats",
                table: "Meeting");

            migrationBuilder.AddColumn<int>(
                name: "Stats",
                table: "User_Meeting",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "Meeting",
                type: "int",
                maxLength: 20,
                nullable: false,
                defaultValue: 0);
        }
    }
}
