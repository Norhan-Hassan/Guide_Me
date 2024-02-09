using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Guide_Me.Migrations
{
    /// <inheritdoc />
    public partial class placeitem2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlaceItem_Places_placeID",
                table: "PlaceItem");

            migrationBuilder.DropForeignKey(
                name: "FK_PlaceItemMedia_PlaceItem_placeItemID",
                table: "PlaceItemMedia");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlaceItem",
                table: "PlaceItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PlaceItemMedia",
                table: "PlaceItemMedia");

            migrationBuilder.RenameTable(
                name: "PlaceItem",
                newName: "placeItem");

            migrationBuilder.RenameTable(
                name: "PlaceItemMedia",
                newName: "placeItemMedias");

            migrationBuilder.RenameIndex(
                name: "IX_PlaceItem_placeID",
                table: "placeItem",
                newName: "IX_placeItem_placeID");

            migrationBuilder.RenameIndex(
                name: "IX_PlaceItemMedia_placeItemID",
                table: "placeItemMedias",
                newName: "IX_placeItemMedias_placeItemID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_placeItem",
                table: "placeItem",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_placeItemMedias",
                table: "placeItemMedias",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_placeItem_Places_placeID",
                table: "placeItem",
                column: "placeID",
                principalTable: "Places",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_placeItemMedias_placeItem_placeItemID",
                table: "placeItemMedias",
                column: "placeItemID",
                principalTable: "placeItem",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_placeItem_Places_placeID",
                table: "placeItem");

            migrationBuilder.DropForeignKey(
                name: "FK_placeItemMedias_placeItem_placeItemID",
                table: "placeItemMedias");

            migrationBuilder.DropPrimaryKey(
                name: "PK_placeItem",
                table: "placeItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_placeItemMedias",
                table: "placeItemMedias");

            migrationBuilder.RenameTable(
                name: "placeItem",
                newName: "PlaceItem");

            migrationBuilder.RenameTable(
                name: "placeItemMedias",
                newName: "PlaceItemMedia");

            migrationBuilder.RenameIndex(
                name: "IX_placeItem_placeID",
                table: "PlaceItem",
                newName: "IX_PlaceItem_placeID");

            migrationBuilder.RenameIndex(
                name: "IX_placeItemMedias_placeItemID",
                table: "PlaceItemMedia",
                newName: "IX_PlaceItemMedia_placeItemID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlaceItem",
                table: "PlaceItem",
                column: "ID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PlaceItemMedia",
                table: "PlaceItemMedia",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_PlaceItem_Places_placeID",
                table: "PlaceItem",
                column: "placeID",
                principalTable: "Places",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PlaceItemMedia_PlaceItem_placeItemID",
                table: "PlaceItemMedia",
                column: "placeItemID",
                principalTable: "PlaceItem",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
