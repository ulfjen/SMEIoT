using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SMEIoT.Infrastructure.Migrations
{
    public partial class AddSensorValue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sensor_values",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    sensor_id = table.Column<long>(nullable: false),
                    value = table.Column<double>(nullable: false),
                    created_at = table.Column<Instant>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sensor_values", x => x.id);
                    table.ForeignKey(
                        name: "fk_sensor_values_sensors_sensor_id",
                        column: x => x.sensor_id,
                        principalTable: "sensors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "id",
                keyValue: 1L,
                column: "concurrency_stamp",
                value: "46df3b92-dcf2-42e5-90e0-b3ed53aaa081");

            migrationBuilder.CreateIndex(
                name: "IX_sensor_values_created_at",
                table: "sensor_values",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_sensor_values_sensor_id",
                table: "sensor_values",
                column: "sensor_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sensor_values");

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "id",
                keyValue: 1L,
                column: "concurrency_stamp",
                value: "0b6ffcc6-712e-41a3-9d79-7ab2e9887ae1");
        }
    }
}
