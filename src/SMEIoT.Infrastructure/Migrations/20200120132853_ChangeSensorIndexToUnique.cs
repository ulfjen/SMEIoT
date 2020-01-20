using Microsoft.EntityFrameworkCore.Migrations;

namespace SMEIoT.Infrastructure.Migrations
{
    public partial class ChangeSensorIndexToUnique : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_sensors_normalized_name_device_id",
                table: "sensors");

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "id",
                keyValue: 1L,
                column: "concurrency_stamp",
                value: "c13d92ee-5425-45da-bd60-7a9eaf2badc4");

            migrationBuilder.CreateIndex(
                name: "IX_sensors_normalized_name_device_id",
                table: "sensors",
                columns: new[] { "normalized_name", "device_id" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_sensors_normalized_name_device_id",
                table: "sensors");

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "id",
                keyValue: 1L,
                column: "concurrency_stamp",
                value: "21e75a0d-126a-4ee7-96c0-de745090a822");

            migrationBuilder.CreateIndex(
                name: "IX_sensors_normalized_name_device_id",
                table: "sensors",
                columns: new[] { "normalized_name", "device_id" });
        }
    }
}
