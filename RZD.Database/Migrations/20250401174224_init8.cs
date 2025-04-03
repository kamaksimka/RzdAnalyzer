using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RZD.Database.Migrations
{
    /// <inheritdoc />
    public partial class init8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_cars_train_id",
                table: "cars");

            migrationBuilder.CreateIndex(
                name: "ix_cars_train_id_car_number",
                table: "cars",
                columns: new[] { "train_id", "car_number" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_cars_train_id_car_number",
                table: "cars");

            migrationBuilder.CreateIndex(
                name: "ix_cars_train_id",
                table: "cars",
                column: "train_id");
        }
    }
}
