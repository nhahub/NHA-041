using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace test.Migrations.Depi
{
    /// <inheritdoc />
    public partial class shelterimp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AnimalId",
                table: "ChatMessages",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_AnimalId",
                table: "ChatMessages",
                column: "AnimalId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_Animals_AnimalId",
                table: "ChatMessages",
                column: "AnimalId",
                principalTable: "Animals",
                principalColumn: "AnimalId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_Animals_AnimalId",
                table: "ChatMessages");

            migrationBuilder.DropIndex(
                name: "IX_ChatMessages_AnimalId",
                table: "ChatMessages");

            migrationBuilder.DropColumn(
                name: "AnimalId",
                table: "ChatMessages");
        }
    }
}
