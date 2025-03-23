using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RZD.Database.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cities",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    express_code = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    region = table.Column<string>(type: "text", nullable: false),
                    foreign_code = table.Column<string>(type: "text", nullable: false),
                    express_codes = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cities", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "entity_types",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_entity_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    email = table.Column<string>(type: "text", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    created_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_sing_in_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "train_stations",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    express_code = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    region = table.Column<string>(type: "text", nullable: false),
                    foreign_code = table.Column<string>(type: "text", nullable: false),
                    city_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_train_stations", x => x.id);
                    table.ForeignKey(
                        name: "fk_train_stations_cities_city_id",
                        column: x => x.city_id,
                        principalTable: "cities",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "entity_histories",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    entity_id = table.Column<long>(type: "bigint", nullable: false),
                    entity_type_id = table.Column<long>(type: "bigint", nullable: false),
                    changed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    changed_fields = table.Column<Dictionary<string, object>>(type: "jsonb", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_entity_histories", x => x.id);
                    table.ForeignKey(
                        name: "fk_entity_histories_entity_types_entity_type_id",
                        column: x => x.entity_type_id,
                        principalTable: "entity_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "feedbacks",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    body = table.Column<string>(type: "text", nullable: false),
                    created_date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_feedbacks", x => x.id);
                    table.ForeignKey(
                        name: "fk_feedbacks_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_roles",
                columns: table => new
                {
                    role_id = table.Column<long>(type: "bigint", nullable: false),
                    users_id = table.Column<long>(type: "bigint", nullable: false),
                    usert_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_roles", x => new { x.role_id, x.users_id });
                    table.ForeignKey(
                        name: "fk_user_roles_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_roles_users_users_id",
                        column: x => x.users_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "trains",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    arrival_date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    arrival_stop_time = table.Column<int>(type: "integer", nullable: false),
                    car_services = table.Column<List<string>>(type: "text[]", nullable: false),
                    departure_date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    departure_stop_time = table.Column<int>(type: "integer", nullable: false),
                    display_train_number = table.Column<string>(type: "text", nullable: false),
                    has_car_transportation_coaches = table.Column<bool>(type: "boolean", nullable: false),
                    has_dynamic_pricing_cars = table.Column<bool>(type: "boolean", nullable: false),
                    has_two_storey_cars = table.Column<bool>(type: "boolean", nullable: false),
                    initial_train_station_code = table.Column<string>(type: "text", nullable: false),
                    is_branded = table.Column<bool>(type: "boolean", nullable: false),
                    is_from_schedule = table.Column<bool>(type: "boolean", nullable: false),
                    is_place_range_allowed = table.Column<bool>(type: "boolean", nullable: false),
                    is_sale_forbidden = table.Column<bool>(type: "boolean", nullable: false),
                    is_suburban = table.Column<bool>(type: "boolean", nullable: false),
                    is_ticket_print_required_for_boarding = table.Column<bool>(type: "boolean", nullable: false),
                    is_tour_package_possible = table.Column<bool>(type: "boolean", nullable: false),
                    is_train_route_allowed = table.Column<bool>(type: "boolean", nullable: false),
                    is_wait_list_available = table.Column<bool>(type: "boolean", nullable: false),
                    local_arrival_date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    local_departure_date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    train_brand_code = table.Column<string>(type: "text", nullable: false),
                    train_description = table.Column<string>(type: "text", nullable: false),
                    train_number = table.Column<string>(type: "text", nullable: false),
                    train_number_to_get_route = table.Column<string>(type: "text", nullable: false),
                    trip_distance = table.Column<int>(type: "integer", nullable: false),
                    trip_duration = table.Column<int>(type: "integer", nullable: false),
                    origin_station_id = table.Column<long>(type: "bigint", nullable: false),
                    destination_station_id = table.Column<long>(type: "bigint", nullable: false),
                    final_train_station_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_trains", x => x.id);
                    table.ForeignKey(
                        name: "fk_trains_train_stations_destination_station_id",
                        column: x => x.destination_station_id,
                        principalTable: "train_stations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_trains_train_stations_final_train_station_id",
                        column: x => x.final_train_station_id,
                        principalTable: "train_stations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_trains_train_stations_origin_station_id",
                        column: x => x.origin_station_id,
                        principalTable: "train_stations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "cars",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    are_places_for_business_travel_booking = table.Column<bool>(type: "boolean", nullable: false),
                    arrival_date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    availability_indication = table.Column<string>(type: "text", nullable: false),
                    car_number = table.Column<string>(type: "text", nullable: false),
                    car_place_type = table.Column<string>(type: "text", nullable: false),
                    car_sub_type = table.Column<string>(type: "text", nullable: false),
                    car_type = table.Column<string>(type: "text", nullable: false),
                    destination_station_code = table.Column<string>(type: "text", nullable: false),
                    free_places = table.Column<string>(type: "text", nullable: false),
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
                    local_arrival_date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    max_price = table.Column<decimal>(type: "numeric", nullable: false),
                    meal_sales_opened_till = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    min_price = table.Column<decimal>(type: "numeric", nullable: false),
                    only_non_refundable_tariff = table.Column<bool>(type: "boolean", nullable: false),
                    passenger_specifying_rules = table.Column<string>(type: "text", nullable: false),
                    place_quantity = table.Column<int>(type: "integer", nullable: false),
                    place_reservation_type = table.Column<string>(type: "text", nullable: false),
                    places_with_conditional_refundable_tariff_quantity = table.Column<int>(type: "integer", nullable: false),
                    service_class = table.Column<string>(type: "text", nullable: false),
                    service_cost = table.Column<decimal>(type: "numeric", nullable: false),
                    services = table.Column<List<string>>(type: "text[]", nullable: false),
                    trip_direction = table.Column<string>(type: "text", nullable: false),
                    train_id = table.Column<long>(type: "bigint", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "routes",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    departure_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    train_number = table.Column<string>(type: "text", nullable: false),
                    origin = table.Column<string>(type: "text", nullable: false),
                    destination = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    origin_name = table.Column<string>(type: "text", nullable: false),
                    destination_name = table.Column<string>(type: "text", nullable: false),
                    train_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_routes", x => x.id);
                    table.ForeignKey(
                        name: "fk_routes_trains_train_id",
                        column: x => x.train_id,
                        principalTable: "trains",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "route_stops",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    actual_movement = table.Column<string>(type: "text", nullable: false),
                    arrival_date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    arrival_time = table.Column<string>(type: "text", nullable: false),
                    departure_date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    departure_time = table.Column<string>(type: "text", nullable: false),
                    local_arrival_date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    local_arrival_time = table.Column<string>(type: "text", nullable: false),
                    local_departure_date_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    local_departure_time = table.Column<string>(type: "text", nullable: false),
                    station_code = table.Column<string>(type: "text", nullable: false),
                    stop_duration = table.Column<int>(type: "integer", nullable: false),
                    time_zone_difference = table.Column<int>(type: "integer", nullable: false),
                    route_id = table.Column<long>(type: "bigint", nullable: false),
                    route_id1 = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_route_stops", x => x.id);
                    table.ForeignKey(
                        name: "fk_route_stops_routes_route_id1",
                        column: x => x.route_id1,
                        principalTable: "routes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_cars_train_id",
                table: "cars",
                column: "train_id");

            migrationBuilder.CreateIndex(
                name: "ix_entity_histories_entity_type_id",
                table: "entity_histories",
                column: "entity_type_id");

            migrationBuilder.CreateIndex(
                name: "ix_feedbacks_user_id",
                table: "feedbacks",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_route_stops_route_id1",
                table: "route_stops",
                column: "route_id1");

            migrationBuilder.CreateIndex(
                name: "ix_routes_train_id",
                table: "routes",
                column: "train_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_train_stations_city_id",
                table: "train_stations",
                column: "city_id");

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
                name: "ix_user_roles_users_id",
                table: "user_roles",
                column: "users_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cars");

            migrationBuilder.DropTable(
                name: "entity_histories");

            migrationBuilder.DropTable(
                name: "feedbacks");

            migrationBuilder.DropTable(
                name: "route_stops");

            migrationBuilder.DropTable(
                name: "user_roles");

            migrationBuilder.DropTable(
                name: "entity_types");

            migrationBuilder.DropTable(
                name: "routes");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "trains");

            migrationBuilder.DropTable(
                name: "train_stations");

            migrationBuilder.DropTable(
                name: "cities");
        }
    }
}
