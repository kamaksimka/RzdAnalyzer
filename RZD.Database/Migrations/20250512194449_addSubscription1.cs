using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RZD.Database.Migrations
{
    /// <inheritdoc />
    public partial class addSubscription1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_any_seat",
                table: "subscriptions",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_any_seat",
                table: "subscriptions");
        }
    }
}
