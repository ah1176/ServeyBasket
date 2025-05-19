using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Survey_Basket.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedIdentityTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "AspNetRoles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AspNetRoles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "IsDefault", "IsDeleted", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "019581ea-b07e-7992-bfe6-933f7e185ae4", "019581ea-b07e-7992-bfe6-934077a144b4", false, false, "Admin", "ADMIN" },
                    { "019581ea-b07e-7992-bfe6-934167315784", "019581ea-b07e-7992-bfe6-934273abfea1", true, false, "Member", "MEMBER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "019581cf-bef8-7a83-bd92-5cd223da4c9b", 0, "019581cf-bef8-7a83-bd92-5cd480454ded", "admin@survey-basket.com", true, "Survey Basket", "Admin", false, null, "ADMIN@SURVEY-BASKET.COM", "ADMIN@SURVEY-BASKET.COM", null, null, false, "49865700D27C420C8A581D9C28C5466B", false, "admin@survey-basket.com" });

            migrationBuilder.InsertData(
                table: "AspNetRoleClaims",
                columns: new[] { "Id", "ClaimType", "ClaimValue", "RoleId" },
                values: new object[,]
                {
                    { 1, "permissions", "polls:read", "019581ea-b07e-7992-bfe6-933f7e185ae4" },
                    { 2, "permissions", "polls:add", "019581ea-b07e-7992-bfe6-933f7e185ae4" },
                    { 3, "permissions", "polls:update", "019581ea-b07e-7992-bfe6-933f7e185ae4" },
                    { 4, "permissions", "polls:delete", "019581ea-b07e-7992-bfe6-933f7e185ae4" },
                    { 5, "permissions", "questions:read", "019581ea-b07e-7992-bfe6-933f7e185ae4" },
                    { 6, "permissions", "questions:add", "019581ea-b07e-7992-bfe6-933f7e185ae4" },
                    { 7, "permissions", "questions:update", "019581ea-b07e-7992-bfe6-933f7e185ae4" },
                    { 8, "permissions", "users:read", "019581ea-b07e-7992-bfe6-933f7e185ae4" },
                    { 9, "permissions", "users:add", "019581ea-b07e-7992-bfe6-933f7e185ae4" },
                    { 10, "permissions", "users:update", "019581ea-b07e-7992-bfe6-933f7e185ae4" },
                    { 11, "permissions", "roles:read", "019581ea-b07e-7992-bfe6-933f7e185ae4" },
                    { 12, "permissions", "roles:add", "019581ea-b07e-7992-bfe6-933f7e185ae4" },
                    { 13, "permissions", "roles:update", "019581ea-b07e-7992-bfe6-933f7e185ae4" },
                    { 14, "permissions", "results:read", "019581ea-b07e-7992-bfe6-933f7e185ae4" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "019581ea-b07e-7992-bfe6-933f7e185ae4", "019581cf-bef8-7a83-bd92-5cd223da4c9b" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "AspNetRoleClaims",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "019581ea-b07e-7992-bfe6-934167315784");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "019581ea-b07e-7992-bfe6-933f7e185ae4", "019581cf-bef8-7a83-bd92-5cd223da4c9b" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "019581ea-b07e-7992-bfe6-933f7e185ae4");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "019581cf-bef8-7a83-bd92-5cd223da4c9b");

            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "AspNetRoles");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AspNetRoles");
        }
    }
}
