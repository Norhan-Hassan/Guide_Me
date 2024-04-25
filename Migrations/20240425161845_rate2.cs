using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Guide_Me.Migrations
{
    /// <inheritdoc />
    public partial class rate2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PlaceId",
                table: "Rating",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Rating_PlaceId",
                table: "Rating",
                column: "PlaceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rating_Places_PlaceId",
                table: "Rating",
                column: "PlaceId",
                principalTable: "Places",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rating_Places_PlaceId",
                table: "Rating");

            migrationBuilder.DropIndex(
                name: "IX_Rating_PlaceId",
                table: "Rating");

            migrationBuilder.DropColumn(
                name: "PlaceId",
                table: "Rating");
        }
    }
}
