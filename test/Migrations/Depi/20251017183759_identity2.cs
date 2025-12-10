using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace test.Migrations.Depi
{
    /// <inheritdoc />
    public partial class identity2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Animals_Users_Userid",
                table: "Animals");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Users_Userid",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Users_Userid",
                table: "Requests");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Users_Useridreq",
                table: "Requests");

            migrationBuilder.AlterColumn<string>(
                name: "Useridreq",
                table: "Requests",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Userid",
                table: "Requests",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Userid",
                table: "Products",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);


            migrationBuilder.AlterColumn<string>(
                name: "Userid",
                table: "Animals",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Animals_AspNetUsers_Userid",
                table: "Animals",
                column: "Userid",
                principalTable: "AspNetUsers",
                principalColumn: "Id");


            migrationBuilder.AddForeignKey(
                name: "FK_Products_AspNetUsers_Userid",
                table: "Products",
                column: "Userid",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_AspNetUsers_Userid",
                table: "Requests",
                column: "Userid",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_AspNetUsers_Useridreq",
                table: "Requests",
                column: "Useridreq",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Animals_AspNetUsers_Userid",
                table: "Animals");

            migrationBuilder.DropForeignKey(
                name: "FK_Animals_Users_UserId",
                table: "Animals");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_AspNetUsers_Userid",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Users_UserId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_AspNetUsers_Userid",
                table: "Requests");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_AspNetUsers_Useridreq",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_Products_UserId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Animals_UserId",
                table: "Animals");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Animals");

            migrationBuilder.AlterColumn<int>(
                name: "Useridreq",
                table: "Requests",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<int>(
                name: "Userid",
                table: "Requests",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<int>(
                name: "Userid",
                table: "Products",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Userid",
                table: "Animals",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Animals_Users_Userid",
                table: "Animals",
                column: "Userid",
                principalTable: "Users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Users_Userid",
                table: "Products",
                column: "Userid",
                principalTable: "Users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Users_Userid",
                table: "Requests",
                column: "Userid",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Users_Useridreq",
                table: "Requests",
                column: "Useridreq",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
