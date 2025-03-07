using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AthliQ.Repository.Data.Migrations
{
    /// <inheritdoc />
    public partial class MtoMIdAddition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "UserClubs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "UserClubs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelated",
                table: "UserClubs",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "UserClubs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ChildTests",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ChildTests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelated",
                table: "ChildTests",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "ChildTests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ChildResults",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ChildResults",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelated",
                table: "ChildResults",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "ChildResults",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "UserClubs");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserClubs");

            migrationBuilder.DropColumn(
                name: "IsDelated",
                table: "UserClubs");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "UserClubs");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ChildTests");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ChildTests");

            migrationBuilder.DropColumn(
                name: "IsDelated",
                table: "ChildTests");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ChildTests");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ChildResults");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ChildResults");

            migrationBuilder.DropColumn(
                name: "IsDelated",
                table: "ChildResults");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ChildResults");
        }
    }
}
