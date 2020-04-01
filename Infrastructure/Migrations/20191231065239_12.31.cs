using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Migrations
{
    public partial class _1231 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "founder",
                table: "Meeting");

            migrationBuilder.DropColumn(
                name: "name",
                table: "Meeting");

            migrationBuilder.DropColumn(
                name: "place",
                table: "Meeting");

            migrationBuilder.RenameColumn(
                name: "content",
                table: "Meeting",
                newName: "Content");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Meeting",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MeetingName",
                table: "Meeting",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDatetime",
                table: "Meeting",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Meeting",
                maxLength: 450,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Meeting");

            migrationBuilder.DropColumn(
                name: "MeetingName",
                table: "Meeting");

            migrationBuilder.DropColumn(
                name: "StartDatetime",
                table: "Meeting");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Meeting");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Meeting",
                newName: "content");

            migrationBuilder.AddColumn<string>(
                name: "founder",
                table: "Meeting",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "Meeting",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "place",
                table: "Meeting",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
