using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace test.Migrations.Depi
{
    /// <inheritdoc />
    public partial class last2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Animals_User_id",
                table: "Animals");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_User_id",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Products",
                newName: "userid");

            migrationBuilder.RenameIndex(
                name: "IX_Products_id",
                table: "Products",
                newName: "IX_Products_userid");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Animals",
                newName: "userid");

            migrationBuilder.RenameIndex(
                name: "IX_Animals_id",
                table: "Animals",
                newName: "IX_Animals_userid");

            migrationBuilder.AddForeignKey(
                name: "FK_Animals_User_userid",
                table: "Animals",
                column: "userid",
                principalTable: "User",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_User_userid",
                table: "Products",
                column: "userid",
                principalTable: "User",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Animals_User_userid",
                table: "Animals");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_User_userid",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "userid",
                table: "Products",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_Products_userid",
                table: "Products",
                newName: "IX_Products_id");

            migrationBuilder.RenameColumn(
                name: "userid",
                table: "Animals",
                newName: "id");

            migrationBuilder.RenameIndex(
                name: "IX_Animals_userid",
                table: "Animals",
                newName: "IX_Animals_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Animals_User_id",
                table: "Animals",
                column: "id",
                principalTable: "User",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_User_id",
                table: "Products",
                column: "id",
                principalTable: "User",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
