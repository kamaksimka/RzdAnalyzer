using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RZD.Database.Migrations
{
    /// <inheritdoc />
    public partial class init22 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_trains_tracked_routes_tracked_route_id",
                table: "trains");

            migrationBuilder.AlterColumn<long>(
                name: "tracked_route_id",
                table: "trains",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_trains_tracked_routes_tracked_route_id",
                table: "trains",
                column: "tracked_route_id",
                principalTable: "tracked_routes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_trains_tracked_routes_tracked_route_id",
                table: "trains");

            migrationBuilder.AlterColumn<long>(
                name: "tracked_route_id",
                table: "trains",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "fk_trains_tracked_routes_tracked_route_id",
                table: "trains",
                column: "tracked_route_id",
                principalTable: "tracked_routes",
                principalColumn: "id");
        }
    }
}
