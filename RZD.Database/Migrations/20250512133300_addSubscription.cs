using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RZD.Database.Migrations
{
    /// <inheritdoc />
    public partial class addSubscription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "subscriptions",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tracked_route_id = table.Column<long>(type: "bigint", nullable: false),
                    start_arrival_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_arrival_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    start_departure_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    end_departure_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    car_types = table.Column<List<string>>(type: "text[]", nullable: false),
                    is_upper_seat = table.Column<bool>(type: "boolean", nullable: false),
                    is_lower_seat = table.Column<bool>(type: "boolean", nullable: false),
                    travel_time_in_minutes = table.Column<int>(type: "integer", nullable: true),
                    min_price = table.Column<decimal>(type: "numeric", nullable: true),
                    max_price = table.Column<decimal>(type: "numeric", nullable: true),
                    car_services = table.Column<List<string>>(type: "text[]", nullable: false),
                    is_complete = table.Column<bool>(type: "boolean", nullable: false),
                    is_delete = table.Column<bool>(type: "boolean", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_subscriptions", x => x.id);
                    table.ForeignKey(
                        name: "fk_subscriptions_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_subscriptions_user_id",
                table: "subscriptions",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "subscriptions");
        }
    }
}
