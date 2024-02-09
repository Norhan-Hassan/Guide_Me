using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Guide_Me.Migrations
{
    /// <inheritdoc />
    public partial class placeitem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlaceItem",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    placeItemName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    placeID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaceItem", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PlaceItem_Places_placeID",
                        column: x => x.placeID,
                        principalTable: "Places",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PlaceItemMedia",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MediaType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MediaContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    placeItemID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaceItemMedia", x => x.ID);
                    table.ForeignKey(
                        name: "FK_PlaceItemMedia_PlaceItem_placeItemID",
                        column: x => x.placeItemID,
                        principalTable: "PlaceItem",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlaceItem_placeID",
                table: "PlaceItem",
                column: "placeID");

            migrationBuilder.CreateIndex(
                name: "IX_PlaceItemMedia_placeItemID",
                table: "PlaceItemMedia",
                column: "placeItemID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlaceItemMedia");

            migrationBuilder.DropTable(
                name: "PlaceItem");
        }
    }
}
