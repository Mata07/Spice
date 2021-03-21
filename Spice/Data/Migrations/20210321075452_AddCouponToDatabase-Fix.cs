using Microsoft.EntityFrameworkCore.Migrations;

namespace Spice.Data.Migrations
{
    public partial class AddCouponToDatabaseFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Coupon");

            migrationBuilder.AddColumn<string>(
                name: "CouponType",
                table: "Coupon",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CouponType",
                table: "Coupon");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Coupon",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
