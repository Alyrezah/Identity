using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Identity.Migrations.Identity
{
    /// <inheritdoc />
    public partial class SiteSettingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SiteSetting",
                columns: table => new
                {
                    Key = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteSetting", x => x.Key);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3DB6B2DF-EADB-41FF-9DE4-434BFFB7C626",
                columns: new[] { "ConcurrencyStamp", "NormalizedUserName", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3f6c2571-0f4a-41d1-a77c-75460f0cbd72", "ALYREZAH", "AQAAAAIAAYagAAAAEMcOa946hkE7NjcZNb0UDPY4ZT3yrClnpZRf1n07tTaB5T2TBGP61b50i8nSU9tpFg==", "94587313-d499-4f90-9582-742e8097859b" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SiteSetting");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "3DB6B2DF-EADB-41FF-9DE4-434BFFB7C626",
                columns: new[] { "ConcurrencyStamp", "NormalizedUserName", "PasswordHash", "SecurityStamp" },
                values: new object[] { "030c27d3-ae92-4863-81d0-f25ffba8e3fa", "ALIREZAH", "AQAAAAIAAYagAAAAEONIGh1YW4CKOn23rE1KQ5LS6uSe4cSiyllAxxL5awbuthTbUrLo8VA35rpFmIU14A==", "7d5c438d-790e-4099-adab-0b51b5be14b0" });
        }
    }
}
