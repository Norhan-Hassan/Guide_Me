using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Guide_Me.Migrations
{
    /// <inheritdoc />
    public partial class rateSugg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rating_Tourist_TouristId",
                table: "Rating");

            migrationBuilder.AlterColumn<string>(
                name: "TouristId",
                table: "Rating",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "RatingSuggestions",
                columns: table => new
                {
                    RateSuggID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Discription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    rateID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RatingSuggestions", x => x.RateSuggID);
                    table.ForeignKey(
                        name: "FK_RatingSuggestions_Rating_rateID",
                        column: x => x.rateID,
                        principalTable: "Rating",
                        principalColumn: "RatingID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RatingSuggestions_rateID",
                table: "RatingSuggestions",
                column: "rateID");

            migrationBuilder.AddForeignKey(
                name: "FK_Rating_Tourist_TouristId",
                table: "Rating",
                column: "TouristId",
                principalTable: "Tourist",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rating_Tourist_TouristId",
                table: "Rating");

            migrationBuilder.DropTable(
                name: "RatingSuggestions");

            migrationBuilder.AlterColumn<string>(
                name: "TouristId",
                table: "Rating",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_Rating_Tourist_TouristId",
                table: "Rating",
                column: "TouristId",
                principalTable: "Tourist",
                principalColumn: "Id");
        }
    }
}
