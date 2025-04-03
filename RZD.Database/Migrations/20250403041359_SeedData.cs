using Microsoft.EntityFrameworkCore.Migrations;
using RZD.Database.Resources;

#nullable disable

namespace RZD.Database.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(ScriptsResource.EntityTypes);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
