using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManokDetectAPI.Migrations
{
    /// <inheritdoc />
    public partial class messagesUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Recipient",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Recipient",
                table: "Messages");
        }
    }
}
