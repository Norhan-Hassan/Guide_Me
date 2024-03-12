using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Guide_Me.Migrations
{
    /// <inheritdoc />
    public partial class updatePlacesofpreferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prefernces_Places_PlaceId",
                table: "Prefernces");

            migrationBuilder.RenameColumn(
                name: "PlaceId",
                table: "Prefernces",
                newName: "PlaceID");

            migrationBuilder.RenameIndex(
                name: "IX_Prefernces_PlaceId",
                table: "Prefernces",
                newName: "IX_Prefernces_PlaceID");

            migrationBuilder.AlterColumn<int>(
                name: "PlaceID",
                table: "Prefernces",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Prefernces_Places_PlaceID",
                table: "Prefernces",
                column: "PlaceID",
                principalTable: "Places",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Prefernces_Places_PlaceID",
                table: "Prefernces");

            migrationBuilder.RenameColumn(
                name: "PlaceID",
                table: "Prefernces",
                newName: "PlaceId");

            migrationBuilder.RenameIndex(
                name: "IX_Prefernces_PlaceID",
                table: "Prefernces",
                newName: "IX_Prefernces_PlaceId");

            migrationBuilder.AlterColumn<int>(
                name: "PlaceId",
                table: "Prefernces",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Prefernces_Places_PlaceId",
                table: "Prefernces",
                column: "PlaceId",
                principalTable: "Places",
                principalColumn: "Id");
        }
    }
}
