using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SMEIoT.Infrastructure.Migrations
{
    public partial class CreateDevices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "devices",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(nullable: false),
                    normalized_name = table.Column<string>(nullable: false),
                    created_at = table.Column<Instant>(nullable: false),
                    updated_at = table.Column<Instant>(nullable: false),
                    authentication_type = table.Column<string>(nullable: false),
                    pre_shared_key = table.Column<string>(nullable: true),
                    connected = table.Column<bool>(nullable: false),
                    connected_at = table.Column<Instant>(nullable: true),
                    last_message_at = table.Column<Instant>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_devices", x => x.id);
                });

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "id",
                keyValue: 1L,
                column: "concurrency_stamp",
                value: "90ed430f-387b-47c5-bff1-91fa4407ccef");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "devices");

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "id",
                keyValue: 1L,
                column: "concurrency_stamp",
                value: "eb66e59b-0ee0-4f01-9050-65033b87a2c3");
        }
    }
}
