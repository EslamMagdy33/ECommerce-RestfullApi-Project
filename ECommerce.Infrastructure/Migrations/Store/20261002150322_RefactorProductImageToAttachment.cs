using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Infrastructure.Migrations.Store
{
    /// <inheritdoc />
    public partial class RefactorProductImageToAttachment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PictureUrl",
                table: "Products");

            migrationBuilder.AddColumn<Guid>(
                name: "PictureAttachmentId",
                table: "Products",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Products_PictureAttachmentId",
                table: "Products",
                column: "PictureAttachmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_UploadedAttachments_PictureAttachmentId",
                table: "Products",
                column: "PictureAttachmentId",
                principalTable: "UploadedAttachments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_UploadedAttachments_PictureAttachmentId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_PictureAttachmentId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "PictureAttachmentId",
                table: "Products");

            migrationBuilder.AddColumn<string>(
                name: "PictureUrl",
                table: "Products",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }
    }
}
