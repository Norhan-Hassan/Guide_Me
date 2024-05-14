using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Guide_Me.Migrations
{
    /// <inheritdoc />
    public partial class imagedelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Tourist");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Tourist",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
