using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VideoGameApi.Migrations
{
    /// <inheritdoc />
    public partial class fixDigitalProductKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DigitalProductKeys_DigitalProducts_DigitalProductId",
                table: "DigitalProductKeys");

            migrationBuilder.AddColumn<int>(
                name: "AssignedToUserId",
                table: "DigitalProductKeys",
                type: "int",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DigitalProductKeys_DigitalProducts_DigitalProductId",
                table: "DigitalProductKeys",
                column: "DigitalProductId",
                principalTable: "DigitalProducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DigitalProductKeys_DigitalProducts_DigitalProductId",
                table: "DigitalProductKeys");

            migrationBuilder.DropColumn(
                name: "AssignedToUserId",
                table: "DigitalProductKeys");

            migrationBuilder.AddForeignKey(
                name: "FK_DigitalProductKeys_DigitalProducts_DigitalProductId",
                table: "DigitalProductKeys",
                column: "DigitalProductId",
                principalTable: "DigitalProducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
