using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RZD.Database.Migrations
{
    /// <inheritdoc />
    public partial class init1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "trip_duration",
                table: "trains",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateIndex(
                name: "ix_trains_train_number_origin_station_id_destination_station_i",
                table: "trains",
                columns: new[] { "train_number", "origin_station_id", "destination_station_id", "departure_date_time" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_trains_train_number_origin_station_id_destination_station_i",
                table: "trains");

            migrationBuilder.AlterColumn<int>(
                name: "trip_duration",
                table: "trains",
                type: "integer",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");
        }
    }
}
