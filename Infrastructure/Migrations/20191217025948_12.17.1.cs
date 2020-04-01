using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Migrations
{
    public partial class _12171 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_meeting_AspNetUsers_UserID",
                table: "meeting");

            migrationBuilder.DropIndex(
                name: "IX_meeting_UserID",
                table: "meeting");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "meeting");

            migrationBuilder.CreateTable(
                name: "User_Meeting",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: false),
                    MeetingId = table.Column<int>(nullable: false),
                    isCreator = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_Meeting", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Meeting_meeting_MeetingId",
                        column: x => x.MeetingId,
                        principalTable: "meeting",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_User_Meeting_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_User_Meeting_MeetingId",
                table: "User_Meeting",
                column: "MeetingId");

            migrationBuilder.CreateIndex(
                name: "IX_User_Meeting_UserId",
                table: "User_Meeting",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "User_Meeting");

            migrationBuilder.AddColumn<string>(
                name: "UserID",
                table: "meeting",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_meeting_UserID",
                table: "meeting",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_meeting_AspNetUsers_UserID",
                table: "meeting",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
