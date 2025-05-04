using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RZD.Database.Migrations
{
    /// <inheritdoc />
    public partial class init24 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "places_with_conditional_refundable_tariff_quantity",
                table: "car_places");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "places_with_conditional_refundable_tariff_quantity",
                table: "car_places",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
