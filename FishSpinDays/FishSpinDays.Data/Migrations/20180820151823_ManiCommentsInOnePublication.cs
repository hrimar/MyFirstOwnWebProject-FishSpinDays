using Microsoft.EntityFrameworkCore.Migrations;

namespace FishSpinDays.Data.Migrations
{
    public partial class ManiCommentsInOnePublication : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PublicationId",
                table: "Comments",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PublicationId1",
                table: "Comments",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_PublicationId1",
                table: "Comments",
                column: "PublicationId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Publications_PublicationId1",
                table: "Comments",
                column: "PublicationId1",
                principalTable: "Publications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Publications_PublicationId1",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_PublicationId1",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "PublicationId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "PublicationId1",
                table: "Comments");
        }
    }
}
