using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RZD.Database.Migrations
{
    /// <inheritdoc />
    public partial class init13 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_cars_train_id_car_number_car_place_type",
                table: "cars");

            migrationBuilder.CreateIndex(
                name: "ix_cars_train_id_car_number_car_place_type_car_type_car_sub_ty",
                table: "cars",
                columns: new[] { "train_id", "car_number", "car_place_type", "car_type", "car_sub_type" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_cars_train_id_car_number_car_place_type_car_type_car_sub_ty",
                table: "cars");

            migrationBuilder.CreateIndex(
                name: "ix_cars_train_id_car_number_car_place_type",
                table: "cars",
                columns: new[] { "train_id", "car_number", "car_place_type" },
                unique: true);
        }
    }
}
