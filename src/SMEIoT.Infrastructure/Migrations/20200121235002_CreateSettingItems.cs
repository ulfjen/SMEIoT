using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

namespace SMEIoT.Infrastructure.Migrations
{
    public partial class CreateSettingItems : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "setting_items",
                columns: table => new
                {
                    name = table.Column<string>(nullable: false),
                    data = table.Column<byte[]>(nullable: false),
                    type = table.Column<string>(nullable: false),
                    created_at = table.Column<Instant>(nullable: false),
                    updated_at = table.Column<Instant>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_setting_items", x => x.name);
                });

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "id",
                keyValue: 1L,
                column: "concurrency_stamp",
                value: "45d60ede-f40b-402a-9a28-a12f0a7b877d");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "setting_items");

            migrationBuilder.UpdateData(
                table: "roles",
                keyColumn: "id",
                keyValue: 1L,
                column: "concurrency_stamp",
                value: "c13d92ee-5425-45da-bd60-7a9eaf2badc4");
        }
    }
}
