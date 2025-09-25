using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FishSpinDays.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangePublicationIdTypeInCommentEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Publications_PublicationId1",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_PublicationId1",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "PublicationId1",
                table: "Comments");

            migrationBuilder.AlterColumn<int>(
                name: "PublicationId",
                table: "Comments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_PublicationId",
                table: "Comments",
                column: "PublicationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Publications_PublicationId",
                table: "Comments",
                column: "PublicationId",
                principalTable: "Publications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Publications_PublicationId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_PublicationId",
                table: "Comments");

            migrationBuilder.AlterColumn<string>(
                name: "PublicationId",
                table: "Comments",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "PublicationId1",
                table: "Comments",
                type: "int",
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
                principalColumn: "Id");
        }
    }
}
