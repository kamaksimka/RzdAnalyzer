using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RZD.Database.Migrations
{
    /// <inheritdoc />
    public partial class init21 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "tracked_route_id",
                table: "trains",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_trains_tracked_route_id",
                table: "trains",
                column: "tracked_route_id");

            migrationBuilder.AddForeignKey(
                name: "fk_trains_tracked_routes_tracked_route_id",
                table: "trains",
                column: "tracked_route_id",
                principalTable: "tracked_routes",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_trains_tracked_routes_tracked_route_id",
                table: "trains");

            migrationBuilder.DropIndex(
                name: "ix_trains_tracked_route_id",
                table: "trains");

            migrationBuilder.DropColumn(
                name: "tracked_route_id",
                table: "trains");
        }
    }
}
