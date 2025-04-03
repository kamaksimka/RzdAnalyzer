using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RZD.Database.Migrations
{
    /// <inheritdoc />
    public partial class init10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "old_field_value",
                table: "entity_histories",
                type: "text",
                nullable: false,
                oldClrType: typeof(object),
                oldType: "jsonb");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<object>(
                name: "old_field_value",
                table: "entity_histories",
                type: "jsonb",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
