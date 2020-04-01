using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Migrations
{
    public partial class _12181 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Meeting_meeting_MeetingId",
                table: "User_Meeting");

            migrationBuilder.DropPrimaryKey(
                name: "PK_meeting",
                table: "meeting");

            migrationBuilder.RenameTable(
                name: "meeting",
                newName: "Meeting");

            migrationBuilder.RenameColumn(
                name: "Founder",
                table: "Meeting",
                newName: "founder");

            migrationBuilder.AlterColumn<string>(
                name: "founder",
                table: "Meeting",
                maxLength: 450,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Meeting",
                table: "Meeting",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Meeting_Meeting_MeetingId",
                table: "User_Meeting",
                column: "MeetingId",
                principalTable: "Meeting",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_User_Meeting_Meeting_MeetingId",
                table: "User_Meeting");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Meeting",
                table: "Meeting");

            migrationBuilder.RenameTable(
                name: "Meeting",
                newName: "meeting");

            migrationBuilder.RenameColumn(
                name: "founder",
                table: "meeting",
                newName: "Founder");

            migrationBuilder.AlterColumn<string>(
                name: "Founder",
                table: "meeting",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 450);

            migrationBuilder.AddPrimaryKey(
                name: "PK_meeting",
                table: "meeting",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_User_Meeting_meeting_MeetingId",
                table: "User_Meeting",
                column: "MeetingId",
                principalTable: "meeting",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
