using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HikeRent.Migrations
{
    /// <inheritdoc />
    public partial class RemoveMaintenanceStock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaintenanceStock",
                table: "GearItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaintenanceStock",
                table: "GearItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
