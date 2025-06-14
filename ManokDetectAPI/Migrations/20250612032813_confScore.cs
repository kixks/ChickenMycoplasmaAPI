using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManokDetectAPI.Migrations
{
    /// <inheritdoc />
    public partial class confScore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "confidenceScore",
                table: "Snapshots",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "confidenceScore",
                table: "Snapshots");
        }
    }
}
