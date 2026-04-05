using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopApi.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditCreatedByUpdatedBy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "CatalogItems",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "CatalogItems",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "CatalogItems",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedBy", "UpdatedBy" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "CatalogItems",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedBy", "UpdatedBy" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "CatalogItems",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedBy", "UpdatedBy" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "CatalogItems",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedBy", "UpdatedBy" },
                values: new object[] { null, null });

            migrationBuilder.UpdateData(
                table: "CatalogItems",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedBy", "UpdatedBy" },
                values: new object[] { null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "CatalogItems");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "CatalogItems");
        }
    }
}
