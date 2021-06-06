using Microsoft.EntityFrameworkCore.Migrations;

namespace StudentskaSluzba.DAL.Migrations
{
    public partial class ClassUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_News_ClassId",
                table: "News",
                column: "ClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_News_Classes_ClassId",
                table: "News",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_News_Classes_ClassId",
                table: "News");

            migrationBuilder.DropIndex(
                name: "IX_News_ClassId",
                table: "News");
        }
    }
}
