using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RZD.Database.Migrations
{
    /// <inheritdoc />
    public partial class init4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "changed_fields",
                table: "entity_histories",
                newName: "old_field_value");

            migrationBuilder.AddColumn<string>(
                name: "field_name",
                table: "entity_histories",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "field_name",
                table: "entity_histories");

            migrationBuilder.RenameColumn(
                name: "old_field_value",
                table: "entity_histories",
                newName: "changed_fields");
        }
    }
}
