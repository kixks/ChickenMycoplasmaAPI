using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ManokDetectAPI.Migrations
{
    /// <inheritdoc />
    public partial class seeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Address", "Email", "MobileNumber", "Name", "Password", "UserType" },
                values: new object[,]
                {
                    { 1, "New York", "dummy1@gmail.com", "1234567890", "dummyFarmer1", "12345", "Farmer" },
                    { 2, "San Angeles", "dummy2@gmail.com", "1234567890", "dummyFarmer2", "123456", "Farmer" },
                    { 3, "Tagum City", "dummyVet1@gmail.com", "1234567890", "dummyVet1", "898989", "Veterinarian" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
