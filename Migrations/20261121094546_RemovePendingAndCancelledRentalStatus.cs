using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HikeRent.Migrations
{
    /// <inheritdoc />
    public partial class RemovePendingAndCancelledRentalStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "UPDATE \"Rentals\" SET \"Status\" = 'Ongoing' WHERE \"Status\" = 'Pending';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
