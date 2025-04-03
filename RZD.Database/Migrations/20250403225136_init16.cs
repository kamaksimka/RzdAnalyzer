using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using RZD.Database.Resources;

#nullable disable

namespace RZD.Database.Migrations
{
    /// <inheritdoc />
    public partial class init16 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cars");

            migrationBuilder.AlterColumn<string>(
                name: "old_field_value",
                table: "entity_histories",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateTable(
                name: "car_places",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    car_number = table.Column<string>(type: "text", nullable: false),
                    car_place_number = table.Column<string>(type: "text", nullable: false),
                    is_booked = table.Column<bool>(type: "boolean", nullable: false),
                    created_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    min_price = table.Column<decimal>(type: "numeric", nullable: false),
                    max_price = table.Column<decimal>(type: "numeric", nullable: false),
                    are_places_for_business_travel_booking = table.Column<bool>(type: "boolean", nullable: false),
                    arrival_date_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    availability_indication = table.Column<string>(type: "text", nullable: true),
                    car_place_type = table.Column<string>(type: "text", nullable: false),
                    car_sub_type = table.Column<string>(type: "text", nullable: true),
                    car_type = table.Column<string>(type: "text", nullable: true),
                    destination_station_code = table.Column<string>(type: "text", nullable: false),
                    has_dynamic_pricing = table.Column<bool>(type: "boolean", nullable: false),
                    has_fss_benefit = table.Column<bool>(type: "boolean", nullable: false),
                    has_gender_cabins = table.Column<bool>(type: "boolean", nullable: false),
                    has_non_refundable_tariff = table.Column<bool>(type: "boolean", nullable: false),
                    has_places_near_babies = table.Column<bool>(type: "boolean", nullable: false),
                    has_places_near_pets = table.Column<bool>(type: "boolean", nullable: false),
                    is_additional_meal_option_possible = table.Column<bool>(type: "boolean", nullable: false),
                    is_additional_passenger_allowed = table.Column<bool>(type: "boolean", nullable: false),
                    is_branded = table.Column<bool>(type: "boolean", nullable: false),
                    is_buffet = table.Column<bool>(type: "boolean", nullable: false),
                    is_car_transportation_coach = table.Column<bool>(type: "boolean", nullable: false),
                    is_child_tariff_type_allowed = table.Column<bool>(type: "boolean", nullable: false),
                    is_for_disabled_persons = table.Column<bool>(type: "boolean", nullable: false),
                    is_meal_option_possible = table.Column<bool>(type: "boolean", nullable: false),
                    is_on_request_meal_option_possible = table.Column<bool>(type: "boolean", nullable: false),
                    is_two_storey = table.Column<bool>(type: "boolean", nullable: false),
                    local_arrival_date_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    meal_sales_opened_till = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    only_non_refundable_tariff = table.Column<bool>(type: "boolean", nullable: false),
                    passenger_specifying_rules = table.Column<string>(type: "text", nullable: true),
                    place_reservation_type = table.Column<string>(type: "text", nullable: true),
                    places_with_conditional_refundable_tariff_quantity = table.Column<int>(type: "integer", nullable: false),
                    service_class = table.Column<string>(type: "text", nullable: true),
                    service_cost = table.Column<decimal>(type: "numeric", nullable: false),
                    services = table.Column<List<string>>(type: "text[]", nullable: false),
                    trip_direction = table.Column<string>(type: "text", nullable: true),
                    train_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_car_places", x => x.id);
                    table.ForeignKey(
                        name: "fk_car_places_trains_train_id",
                        column: x => x.train_id,
                        principalTable: "trains",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_car_places_train_id_car_number_car_place_number",
                table: "car_places",
                columns: new[] { "train_id", "car_number", "car_place_number" },
                unique: true);

            migrationBuilder.Sql(ScriptsResource.EntityTypes);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "car_places");

            migrationBuilder.AlterColumn<string>(
                name: "old_field_value",
                table: "entity_histories",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "cars",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    train_id = table.Column<long>(type: "bigint", nullable: false),
                    are_places_for_business_travel_booking = table.Column<bool>(type: "boolean", nullable: false),
                    arrival_date_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    availability_indication = table.Column<string>(type: "text", nullable: true),
                    car_number = table.Column<string>(type: "text", nullable: false),
                    car_place_type = table.Column<string>(type: "text", nullable: false),
                    car_sub_type = table.Column<string>(type: "text", nullable: true),
                    car_type = table.Column<string>(type: "text", nullable: true),
                    created_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    destination_station_code = table.Column<string>(type: "text", nullable: false),
                    free_places = table.Column<string>(type: "text", nullable: true),
                    has_dynamic_pricing = table.Column<bool>(type: "boolean", nullable: false),
                    has_fss_benefit = table.Column<bool>(type: "boolean", nullable: false),
                    has_gender_cabins = table.Column<bool>(type: "boolean", nullable: false),
                    has_non_refundable_tariff = table.Column<bool>(type: "boolean", nullable: false),
                    has_places_near_babies = table.Column<bool>(type: "boolean", nullable: false),
                    has_places_near_pets = table.Column<bool>(type: "boolean", nullable: false),
                    is_additional_meal_option_possible = table.Column<bool>(type: "boolean", nullable: false),
                    is_additional_passenger_allowed = table.Column<bool>(type: "boolean", nullable: false),
                    is_branded = table.Column<bool>(type: "boolean", nullable: false),
                    is_buffet = table.Column<bool>(type: "boolean", nullable: false),
                    is_car_transportation_coach = table.Column<bool>(type: "boolean", nullable: false),
                    is_child_tariff_type_allowed = table.Column<bool>(type: "boolean", nullable: false),
                    is_for_disabled_persons = table.Column<bool>(type: "boolean", nullable: false),
                    is_meal_option_possible = table.Column<bool>(type: "boolean", nullable: false),
                    is_on_request_meal_option_possible = table.Column<bool>(type: "boolean", nullable: false),
                    is_two_storey = table.Column<bool>(type: "boolean", nullable: false),
                    local_arrival_date_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    max_price = table.Column<decimal>(type: "numeric", nullable: false),
                    meal_sales_opened_till = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    min_price = table.Column<decimal>(type: "numeric", nullable: false),
                    only_non_refundable_tariff = table.Column<bool>(type: "boolean", nullable: false),
                    passenger_specifying_rules = table.Column<string>(type: "text", nullable: true),
                    place_quantity = table.Column<int>(type: "integer", nullable: false),
                    place_reservation_type = table.Column<string>(type: "text", nullable: true),
                    places_with_conditional_refundable_tariff_quantity = table.Column<int>(type: "integer", nullable: false),
                    service_class = table.Column<string>(type: "text", nullable: true),
                    service_cost = table.Column<decimal>(type: "numeric", nullable: false),
                    services = table.Column<List<string>>(type: "text[]", nullable: false),
                    trip_direction = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cars", x => x.id);
                    table.ForeignKey(
                        name: "fk_cars_trains_train_id",
                        column: x => x.train_id,
                        principalTable: "trains",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_cars_train_id_car_number_car_place_type_car_type_car_sub_ty",
                table: "cars",
                columns: new[] { "train_id", "car_number", "car_place_type", "car_type", "car_sub_type", "service_class" },
                unique: true);
        }
    }
}
