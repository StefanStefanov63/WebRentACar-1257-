using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebRentACar.Data.Migrations
{
    /// <inheritdoc />
    public partial class uniqueUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_CarPictures_PictureUrl",
                table: "CarPictures",
                column: "PictureUrl",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CarPictures_PictureUrl",
                table: "CarPictures");
        }
    }
}
