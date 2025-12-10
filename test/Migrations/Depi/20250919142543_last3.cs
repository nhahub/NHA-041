using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace test.Migrations.Depi
{
    /// <inheritdoc />
    public partial class last3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Animals_User_userid",
                table: "Animals");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_User_userid",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_userid",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Animals_userid",
                table: "Animals");

            migrationBuilder.DropPrimaryKey(
                name: "PK_User",
                table: "User");

            migrationBuilder.RenameTable(
                name: "User",
                newName: "Users");

            migrationBuilder.RenameColumn(
                name: "userid",
                table: "Products",
                newName: "Userid");

            migrationBuilder.RenameColumn(
                name: "userid",
                table: "Animals",
                newName: "Userid");

            migrationBuilder.AlterColumn<int>(
                name: "Userid",
                table: "Products",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "id",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "Userid",
                table: "Animals",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "id",
                table: "Animals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_Products_id",
                table: "Products",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_Animals_id",
                table: "Animals",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_Animals_Users_id",
                table: "Animals",
                column: "id",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Users_id",
                table: "Products",
                column: "id",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Animals_Users_id",
                table: "Animals");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Users_id",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_id",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Animals_id",
                table: "Animals");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "id",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "id",
                table: "Animals");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "User");

            migrationBuilder.RenameColumn(
                name: "Userid",
                table: "Products",
                newName: "userid");

            migrationBuilder.RenameColumn(
                name: "Userid",
                table: "Animals",
                newName: "userid");

            migrationBuilder.AlterColumn<int>(
                name: "userid",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "userid",
                table: "Animals",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_User",
                table: "User",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_Products_userid",
                table: "Products",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "IX_Animals_userid",
                table: "Animals",
                column: "userid");

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
    }
}
