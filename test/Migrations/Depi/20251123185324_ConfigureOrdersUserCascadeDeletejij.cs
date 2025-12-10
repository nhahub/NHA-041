using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace test.Migrations.Depi
{
    /// <inheritdoc />
    public partial class ConfigureOrdersUserCascadeDeletejij : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "anim_rec_fk",
                table: "medical_record");

            migrationBuilder.DropForeignKey(
                name: "vac_rec_fk",
                table: "vaccination_needed");

            migrationBuilder.AddForeignKey(
                name: "anim_rec_fk",
                table: "medical_record",
                column: "animalid",
                principalTable: "Animals",
                principalColumn: "AnimalId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "vac_rec_fk",
                table: "vaccination_needed",
                column: "medicalid",
                principalTable: "medical_record",
                principalColumn: "recordid",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "anim_rec_fk",
                table: "medical_record");

            migrationBuilder.DropForeignKey(
                name: "vac_rec_fk",
                table: "vaccination_needed");

            migrationBuilder.AddForeignKey(
                name: "anim_rec_fk",
                table: "medical_record",
                column: "animalid",
                principalTable: "Animals",
                principalColumn: "AnimalId");

            migrationBuilder.AddForeignKey(
                name: "vac_rec_fk",
                table: "vaccination_needed",
                column: "medicalid",
                principalTable: "medical_record",
                principalColumn: "recordid");
        }
    }
}
