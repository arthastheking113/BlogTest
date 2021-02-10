using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BlogTest.Data.Migrations
{
    public partial class Initial2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "PostCategory",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "ImageData",
                table: "PostCategory",
                type: "bytea",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "PostCategory");

            migrationBuilder.DropColumn(
                name: "ImageData",
                table: "PostCategory");
        }
    }
}
