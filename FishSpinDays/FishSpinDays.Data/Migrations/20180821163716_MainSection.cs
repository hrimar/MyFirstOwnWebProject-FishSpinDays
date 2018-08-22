using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FishSpinDays.Data.Migrations
{
    public partial class MainSection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sections_Name",
                table: "Sections");

            migrationBuilder.AddColumn<int>(
                name: "MainSectionId",
                table: "Sections",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "MainSections",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MainSections", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sections_MainSectionId",
                table: "Sections",
                column: "MainSectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sections_MainSections_MainSectionId",
                table: "Sections",
                column: "MainSectionId",
                principalTable: "MainSections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sections_MainSections_MainSectionId",
                table: "Sections");

            migrationBuilder.DropTable(
                name: "MainSections");

            migrationBuilder.DropIndex(
                name: "IX_Sections_MainSectionId",
                table: "Sections");

            migrationBuilder.DropColumn(
                name: "MainSectionId",
                table: "Sections");

            migrationBuilder.CreateIndex(
                name: "IX_Sections_Name",
                table: "Sections",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");
        }
    }
}
