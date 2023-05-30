using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApi.Data.Migrations
{
    public partial class Migration20235202204 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TB_Book",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 128, nullable: true),
                    Author = table.Column<string>(maxLength: 64, nullable: true),
                    ISBN = table.Column<string>(maxLength: 32, nullable: true),
                    Publisher = table.Column<string>(maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_Book", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TB_Inventory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<int>(nullable: false),
                    Bookid = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TB_Inventory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TB_Inventory_TB_Book_Bookid",
                        column: x => x.Bookid,
                        principalTable: "TB_Book",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TB_Inventory_Bookid",
                table: "TB_Inventory",
                column: "Bookid",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TB_Inventory");

            migrationBuilder.DropTable(
                name: "TB_Book");
        }
    }
}
