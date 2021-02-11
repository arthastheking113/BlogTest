﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BlogTest.Data.Migrations
{
    public partial class Intial4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "AspNetUsers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "ImageData",
                table: "AspNetUsers",
                type: "bytea",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ImageData",
                table: "AspNetUsers");
        }
    }
}
