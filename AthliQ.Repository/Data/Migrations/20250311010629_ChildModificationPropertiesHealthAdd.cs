using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AthliQ.Repository.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChildModificationPropertiesHealthAdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsAgreeHealthPolicies",
                table: "Children",
                newName: "IsNormalBloodTest");

            migrationBuilder.AddColumn<bool>(
                name: "IsAgreeDoctorApproval",
                table: "Children",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAgreeDoctorApproval",
                table: "Children");

            migrationBuilder.RenameColumn(
                name: "IsNormalBloodTest",
                table: "Children",
                newName: "IsAgreeHealthPolicies");
        }
    }
}
