using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToPropertyListing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "PropertyListings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PropertyListings_UserId",
                table: "PropertyListings",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_PropertyListings_AspNetUsers_UserId",
                table: "PropertyListings",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PropertyListings_AspNetUsers_UserId",
                table: "PropertyListings");

            migrationBuilder.DropIndex(
                name: "IX_PropertyListings_UserId",
                table: "PropertyListings");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "PropertyListings");
        }
    }
}
