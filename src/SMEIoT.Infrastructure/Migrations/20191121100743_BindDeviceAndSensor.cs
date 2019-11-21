using Microsoft.EntityFrameworkCore.Migrations;

namespace SMEIoT.Infrastructure.Migrations
{
    public partial class BindDeviceAndSensor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "device_id",
                table: "sensors",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "id",
                keyValue: 1L,
                column: "concurrency_stamp",
                value: "0b6ffcc6-712e-41a3-9d79-7ab2e9887ae1");

            migrationBuilder.CreateIndex(
                name: "ix_sensors_device_id",
                table: "sensors",
                column: "device_id");

            migrationBuilder.CreateIndex(
                name: "IX_sensors_normalized_name_device_id",
                table: "sensors",
                columns: new[] { "normalized_name", "device_id" });

            migrationBuilder.CreateIndex(
                name: "IX_devices_normalized_name",
                table: "devices",
                column: "normalized_name");

            migrationBuilder.AddForeignKey(
                name: "fk_sensors_devices_device_id",
                table: "sensors",
                column: "device_id",
                principalTable: "devices",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_sensors_devices_device_id",
                table: "sensors");

            migrationBuilder.DropIndex(
                name: "ix_sensors_device_id",
                table: "sensors");

            migrationBuilder.DropIndex(
                name: "IX_sensors_normalized_name_device_id",
                table: "sensors");

            migrationBuilder.DropIndex(
                name: "IX_devices_normalized_name",
                table: "devices");

            migrationBuilder.DropColumn(
                name: "device_id",
                table: "sensors");

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "id",
                keyValue: 1L,
                column: "concurrency_stamp",
                value: "90ed430f-387b-47c5-bff1-91fa4407ccef");
        }
    }
}
