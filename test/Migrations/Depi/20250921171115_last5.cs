using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace test.Migrations.Depi
{
    /// <inheritdoc />
    public partial class last5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "Req_to",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "id",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "id",
                table: "Animals");

            migrationBuilder.RenameColumn(
                name: "userid",
                table: "Requests",
                newName: "Userid");

            migrationBuilder.RenameColumn(
                name: "AnimalID",
                table: "Requests",
                newName: "AnimalId");

            migrationBuilder.AlterColumn<int>(
                name: "Userid",
                table: "Requests",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldMaxLength: 450,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Useridreq",
                table: "Requests",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Requests_AnimalId",
                table: "Requests",
                column: "AnimalId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_Userid",
                table: "Requests",
                column: "Userid");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_Useridreq",
                table: "Requests",
                column: "Useridreq");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Userid",
                table: "Products",
                column: "Userid");

            migrationBuilder.CreateIndex(
                name: "IX_Animals_Userid",
                table: "Animals",
                column: "Userid");

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
                name: "FK_Requests_Animals_AnimalId",
                table: "Requests",
                column: "AnimalId",
                principalTable: "Animals",
                principalColumn: "animalID");

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Users_Userid",
                table: "Requests",
                column: "Userid",
                principalTable: "Users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Users_Useridreq",
                table: "Requests",
                column: "Useridreq",
                principalTable: "Users",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Animals_Users_Userid",
                table: "Animals");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Users_Userid",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Animals_AnimalId",
                table: "Requests");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Users_Userid",
                table: "Requests");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Users_Useridreq",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_Requests_AnimalId",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_Requests_Userid",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_Requests_Useridreq",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_Products_Userid",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Animals_Userid",
                table: "Animals");

            migrationBuilder.DropColumn(
                name: "Useridreq",
                table: "Requests");

            migrationBuilder.RenameColumn(
                name: "Userid",
                table: "Requests",
                newName: "userid");

            migrationBuilder.RenameColumn(
                name: "AnimalId",
                table: "Requests",
                newName: "AnimalID");

            migrationBuilder.AlterColumn<string>(
                name: "userid",
                table: "Requests",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Req_to",
                table: "Requests",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "id",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "id",
                table: "Animals",
                type: "int",
                nullable: false,
                defaultValue: 0);

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
    }
}
