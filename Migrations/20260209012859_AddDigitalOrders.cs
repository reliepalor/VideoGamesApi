using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VideoGameApi.Migrations
{
    /// <inheritdoc />
    public partial class AddDigitalOrders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DigitalProductKeys_DigitalProducts_DigitalProductId",
                table: "DigitalProductKeys");

            migrationBuilder.CreateTable(
                name: "DigitalOrders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    DigitalProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DigitalOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DigitalOrders_DigitalProducts_DigitalProductId",
                        column: x => x.DigitalProductId,
                        principalTable: "DigitalProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DigitalOrders_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DigitalOrders_Users_UserId1",
                        column: x => x.UserId1,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DigitalOrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DigitalOrderId = table.Column<int>(type: "int", nullable: false),
                    DigitalProductKeyId = table.Column<int>(type: "int", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DigitalOrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DigitalOrderItems_DigitalOrders_DigitalOrderId",
                        column: x => x.DigitalOrderId,
                        principalTable: "DigitalOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DigitalOrderItems_DigitalProductKeys_DigitalProductKeyId",
                        column: x => x.DigitalProductKeyId,
                        principalTable: "DigitalProductKeys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DigitalOrderItems_DigitalOrderId",
                table: "DigitalOrderItems",
                column: "DigitalOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_DigitalOrderItems_DigitalProductKeyId",
                table: "DigitalOrderItems",
                column: "DigitalProductKeyId");

            migrationBuilder.CreateIndex(
                name: "IX_DigitalOrders_DigitalProductId",
                table: "DigitalOrders",
                column: "DigitalProductId");

            migrationBuilder.CreateIndex(
                name: "IX_DigitalOrders_UserId",
                table: "DigitalOrders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_DigitalOrders_UserId1",
                table: "DigitalOrders",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_DigitalProductKeys_DigitalProducts_DigitalProductId",
                table: "DigitalProductKeys",
                column: "DigitalProductId",
                principalTable: "DigitalProducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DigitalProductKeys_DigitalProducts_DigitalProductId",
                table: "DigitalProductKeys");

            migrationBuilder.DropTable(
                name: "DigitalOrderItems");

            migrationBuilder.DropTable(
                name: "DigitalOrders");

            migrationBuilder.AddForeignKey(
                name: "FK_DigitalProductKeys_DigitalProducts_DigitalProductId",
                table: "DigitalProductKeys",
                column: "DigitalProductId",
                principalTable: "DigitalProducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
