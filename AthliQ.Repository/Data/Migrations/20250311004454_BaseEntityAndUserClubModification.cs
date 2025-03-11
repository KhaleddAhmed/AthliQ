using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AthliQ.Repository.Data.Migrations
{
    /// <inheritdoc />
    public partial class BaseEntityAndUserClubModification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Id",
                table: "UserClubs");

            migrationBuilder.RenameColumn(
                name: "IsDelated",
                table: "UserClubs",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "IsDelated",
                table: "Tests",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "IsDelated",
                table: "Sports",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "IsDelated",
                table: "ChildTests",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "IsDelated",
                table: "ChildResults",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "IsDelated",
                table: "Children",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "IsDelated",
                table: "Categories",
                newName: "IsDeleted");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "UserClubs",
                newName: "IsDelated");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "Tests",
                newName: "IsDelated");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "Sports",
                newName: "IsDelated");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "ChildTests",
                newName: "IsDelated");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "ChildResults",
                newName: "IsDelated");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "Children",
                newName: "IsDelated");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "Categories",
                newName: "IsDelated");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "UserClubs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
