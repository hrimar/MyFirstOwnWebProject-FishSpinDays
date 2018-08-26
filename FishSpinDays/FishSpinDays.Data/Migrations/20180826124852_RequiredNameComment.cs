using Microsoft.EntityFrameworkCore.Migrations;

namespace FishSpinDays.Data.Migrations
{
    public partial class RequiredNameComment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "Comments",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "Comments",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
