using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SMEIoT.Infrastructure.Migrations
{
    public partial class ChangeSensorValueKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_sensor_values",
                table: "sensor_values");

            migrationBuilder.DropIndex(
                name: "ix_sensor_values_sensor_id",
                table: "sensor_values");

            migrationBuilder.DropColumn(
                name: "id",
                table: "sensor_values");

            migrationBuilder.AddPrimaryKey(
                name: "PK_sensor_values",
                table: "sensor_values",
                columns: new[] { "sensor_id", "created_at" });

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "id",
                keyValue: 1L,
                column: "concurrency_stamp",
                value: "21e75a0d-126a-4ee7-96c0-de745090a822");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_sensor_values",
                table: "sensor_values");

            migrationBuilder.AddColumn<long>(
                name: "id",
                table: "sensor_values",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddPrimaryKey(
                name: "pk_sensor_values",
                table: "sensor_values",
                column: "id");

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "id",
                keyValue: 1L,
                column: "concurrency_stamp",
                value: "4d49acee-0341-4c00-9982-651ba49cf735");

            migrationBuilder.CreateIndex(
                name: "ix_sensor_values_sensor_id",
                table: "sensor_values",
                column: "sensor_id");
        }
    }
}
