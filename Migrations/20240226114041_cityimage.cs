using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Guide_Me.Migrations
{
    /// <inheritdoc />
    public partial class cityimage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CityImage",
                table: "Cities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CityImage",
                table: "Cities");
        }
    }
}
