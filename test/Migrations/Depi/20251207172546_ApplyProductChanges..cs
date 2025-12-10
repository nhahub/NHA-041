using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace test.Migrations.Depi
{
    /// <inheritdoc />
    public partial class ApplyProductChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. إضافة عمود Name (الاسم)
            // الجدول اسمه Products (بالجمع)
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Products",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            // 2. تعديل السعر (Price)
            // لاحظي: استخدمنا اسم العمود "price" (سمول) زي ما هو مكتوب في الموديل بتاعك
            migrationBuilder.AlterColumn<decimal>(
                name: "price",
                table: "Products",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            // 3. تعديل الكمية (Quantity)
            // لاحظي: استخدمنا اسم العمود "quantity" (سمول)
            migrationBuilder.AlterColumn<int>(
                name: "quantity",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }


        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
