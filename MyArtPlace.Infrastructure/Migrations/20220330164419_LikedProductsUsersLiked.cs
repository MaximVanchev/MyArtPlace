using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyArtPlace.Migrations
{
    public partial class LikedProductsUsersLiked : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserProduct",
                columns: table => new
                {
                    LikedProductsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsersLikedId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MyArtPlaceUserProduct", x => new { x.LikedProductsId, x.UsersLikedId });
                    table.ForeignKey(
                        name: "FK_MyArtPlaceUserProduct_AspNetUsers_UsersLikedId",
                        column: x => x.UsersLikedId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MyArtPlaceUserProduct_Products_LikedProductsId",
                        column: x => x.LikedProductsId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MyArtPlaceUserProduct_UsersLikedId",
                table: "MyArtPlaceUserProduct",
                column: "UsersLikedId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MyArtPlaceUserProduct");
        }
    }
}
