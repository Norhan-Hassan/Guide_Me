using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Guide_Me.Migrations
{
    /// <inheritdoc />
    public partial class updatepreferncesName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prefernces_AspNetUsers_TouristID",
                table: "Prefernces");

            migrationBuilder.DropForeignKey(
                name: "FK_Prefernces_Places_PlaceID",
                table: "Prefernces");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Prefernces",
                table: "Prefernces");

            migrationBuilder.RenameTable(
                name: "Prefernces",
                newName: "Favorites");

            migrationBuilder.RenameIndex(
                name: "IX_Prefernces_TouristID",
                table: "Favorites",
                newName: "IX_Favorites_TouristID");

            migrationBuilder.RenameIndex(
                name: "IX_Prefernces_PlaceID",
                table: "Favorites",
                newName: "IX_Favorites_PlaceID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Favorites",
                table: "Favorites",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_AspNetUsers_TouristID",
                table: "Favorites",
                column: "TouristID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Favorites_Places_PlaceID",
                table: "Favorites",
                column: "PlaceID",
                principalTable: "Places",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_AspNetUsers_TouristID",
                table: "Favorites");

            migrationBuilder.DropForeignKey(
                name: "FK_Favorites_Places_PlaceID",
                table: "Favorites");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Favorites",
                table: "Favorites");

            migrationBuilder.RenameTable(
                name: "Favorites",
                newName: "Prefernces");

            migrationBuilder.RenameIndex(
                name: "IX_Favorites_TouristID",
                table: "Prefernces",
                newName: "IX_Prefernces_TouristID");

            migrationBuilder.RenameIndex(
                name: "IX_Favorites_PlaceID",
                table: "Prefernces",
                newName: "IX_Prefernces_PlaceID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Prefernces",
                table: "Prefernces",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Prefernces_AspNetUsers_TouristID",
                table: "Prefernces",
                column: "TouristID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Prefernces_Places_PlaceID",
                table: "Prefernces",
                column: "PlaceID",
                principalTable: "Places",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
