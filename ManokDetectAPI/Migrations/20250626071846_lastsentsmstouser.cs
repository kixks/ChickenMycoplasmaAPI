using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManokDetectAPI.Migrations
{
    /// <inheritdoc />
    public partial class lastsentsmstouser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastSmsSentAt",
                table: "Users",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastSmsSentAt",
                table: "Users");
        }
    }
}
