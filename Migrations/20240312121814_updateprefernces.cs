using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Guide_Me.Migrations
{
    /// <inheritdoc />
    public partial class updateprefernces : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PlaceId",
                table: "Prefernces",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TouristID",
                table: "Prefernces",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Prefernces_PlaceId",
                table: "Prefernces",
                column: "PlaceId");

            migrationBuilder.CreateIndex(
                name: "IX_Prefernces_TouristID",
                table: "Prefernces",
                column: "TouristID");

            migrationBuilder.AddForeignKey(
                name: "FK_Prefernces_AspNetUsers_TouristID",
                table: "Prefernces",
                column: "TouristID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Prefernces_Places_PlaceId",
                table: "Prefernces",
                column: "PlaceId",
                principalTable: "Places",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prefernces_AspNetUsers_TouristID",
                table: "Prefernces");

            migrationBuilder.DropForeignKey(
                name: "FK_Prefernces_Places_PlaceId",
                table: "Prefernces");

            migrationBuilder.DropIndex(
                name: "IX_Prefernces_PlaceId",
                table: "Prefernces");

            migrationBuilder.DropIndex(
                name: "IX_Prefernces_TouristID",
                table: "Prefernces");

            migrationBuilder.DropColumn(
                name: "PlaceId",
                table: "Prefernces");

            migrationBuilder.DropColumn(
                name: "TouristID",
                table: "Prefernces");
        }
    }
}
