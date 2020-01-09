using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

namespace SMEIoT.Infrastructure.Migrations
{
    public partial class AddMqttStampToSensor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "connected",
                table: "sensors",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Instant>(
                name: "connected_at",
                table: "sensors",
                nullable: true);

            migrationBuilder.AddColumn<Instant>(
                name: "last_message_at",
                table: "sensors",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "id",
                keyValue: 1L,
                column: "concurrency_stamp",
                value: "4d49acee-0341-4c00-9982-651ba49cf735");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "connected",
                table: "sensors");

            migrationBuilder.DropColumn(
                name: "connected_at",
                table: "sensors");

            migrationBuilder.DropColumn(
                name: "last_message_at",
                table: "sensors");

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "id",
                keyValue: 1L,
                column: "concurrency_stamp",
                value: "46df3b92-dcf2-42e5-90e0-b3ed53aaa081");
        }
    }
}
