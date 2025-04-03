using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RZD.Database.Migrations
{
    /// <inheritdoc />
    public partial class init7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_trains_train_stations_destination_station_id",
                table: "trains");

            migrationBuilder.DropForeignKey(
                name: "fk_trains_train_stations_final_train_station_id",
                table: "trains");

            migrationBuilder.DropForeignKey(
                name: "fk_trains_train_stations_origin_station_id",
                table: "trains");

            migrationBuilder.DropIndex(
                name: "ix_trains_destination_station_id",
                table: "trains");

            migrationBuilder.DropIndex(
                name: "ix_trains_final_train_station_id",
                table: "trains");

            migrationBuilder.DropIndex(
                name: "ix_trains_origin_station_id",
                table: "trains");

            migrationBuilder.DropIndex(
                name: "ix_trains_train_number_origin_station_id_destination_station_i",
                table: "trains");

            migrationBuilder.DropColumn(
                name: "destination_station_id",
                table: "trains");

            migrationBuilder.DropColumn(
                name: "final_train_station_id",
                table: "trains");

            migrationBuilder.DropColumn(
                name: "origin_station_id",
                table: "trains");

            migrationBuilder.AddColumn<string>(
                name: "destination_station_code",
                table: "trains",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "final_train_station_code",
                table: "trains",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "origin_station_code",
                table: "trains",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "ix_trains_train_number_origin_station_code_destination_station",
                table: "trains",
                columns: new[] { "train_number", "origin_station_code", "destination_station_code", "departure_date_time" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_trains_train_number_origin_station_code_destination_station",
                table: "trains");

            migrationBuilder.DropColumn(
                name: "destination_station_code",
                table: "trains");

            migrationBuilder.DropColumn(
                name: "final_train_station_code",
                table: "trains");

            migrationBuilder.DropColumn(
                name: "origin_station_code",
                table: "trains");

            migrationBuilder.AddColumn<long>(
                name: "destination_station_id",
                table: "trains",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "final_train_station_id",
                table: "trains",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "origin_station_id",
                table: "trains",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "ix_trains_destination_station_id",
                table: "trains",
                column: "destination_station_id");

            migrationBuilder.CreateIndex(
                name: "ix_trains_final_train_station_id",
                table: "trains",
                column: "final_train_station_id");

            migrationBuilder.CreateIndex(
                name: "ix_trains_origin_station_id",
                table: "trains",
                column: "origin_station_id");

            migrationBuilder.CreateIndex(
                name: "ix_trains_train_number_origin_station_id_destination_station_i",
                table: "trains",
                columns: new[] { "train_number", "origin_station_id", "destination_station_id", "departure_date_time" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_trains_train_stations_destination_station_id",
                table: "trains",
                column: "destination_station_id",
                principalTable: "train_stations",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_trains_train_stations_final_train_station_id",
                table: "trains",
                column: "final_train_station_id",
                principalTable: "train_stations",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_trains_train_stations_origin_station_id",
                table: "trains",
                column: "origin_station_id",
                principalTable: "train_stations",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
