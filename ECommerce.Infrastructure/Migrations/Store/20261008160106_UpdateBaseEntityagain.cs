using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Infrastructure.Migrations.Store
{
    /// <inheritdoc />
    public partial class UpdateBaseEntityagain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsHardDeleted",
                table: "UserAddresses");

            migrationBuilder.DropColumn(
                name: "IsHardDeleted",
                table: "UploadedAttachments");

            migrationBuilder.DropColumn(
                name: "IsHardDeleted",
                table: "Types");

            migrationBuilder.DropColumn(
                name: "IsHardDeleted",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsHardDeleted",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "IsHardDeleted",
                table: "DeliveryMethods");

            migrationBuilder.DropColumn(
                name: "IsHardDeleted",
                table: "Brands");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsHardDeleted",
                table: "UserAddresses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsHardDeleted",
                table: "UploadedAttachments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsHardDeleted",
                table: "Types",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsHardDeleted",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsHardDeleted",
                table: "Orders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsHardDeleted",
                table: "DeliveryMethods",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsHardDeleted",
                table: "Brands",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
