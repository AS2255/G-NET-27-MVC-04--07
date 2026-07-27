using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymManagement.DAL.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemovedNameFromSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Sessions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Sessions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
