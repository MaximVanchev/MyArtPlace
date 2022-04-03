using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyArtPlace.Migrations
{
    public partial class UserCartOrderId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UsersCarts",
                table: "UsersCarts");

            migrationBuilder.AddColumn<Guid>(
                name: "OrderId",
                table: "UsersCarts",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "InCart",
                table: "UsersCarts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsersCarts",
                table: "UsersCarts",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersCarts_UserId",
                table: "UsersCarts",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UsersCarts",
                table: "UsersCarts");

            migrationBuilder.DropIndex(
                name: "IX_UsersCarts_UserId",
                table: "UsersCarts");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "UsersCarts");

            migrationBuilder.DropColumn(
                name: "InCart",
                table: "UsersCarts");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsersCarts",
                table: "UsersCarts",
                columns: new[] { "UserId", "ProductId" });
        }
    }
}
