using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopApi.Migrations
{
    /// <inheritdoc />
    public partial class ApplySchemaConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_CatalogItem_AvailableStock",
                table: "CatalogItems",
                sql: "\"AvailableStock\" >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CatalogItem_Price",
                table: "CatalogItems",
                sql: "\"Price\" > 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_CatalogItem_AvailableStock",
                table: "CatalogItems");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CatalogItem_Price",
                table: "CatalogItems");
        }
    }
}
