using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Text;

#nullable disable

namespace MovieApp.Migrations
{
    /// <inheritdoc />
    public partial class SeedIdentityData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create Admin role
            var adminRoleId = Guid.NewGuid().ToString();
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                values: new object[] { adminRoleId, "Admin", "ADMIN", Guid.NewGuid().ToString() }
            );

            // Create User role
            var userRoleId = Guid.NewGuid().ToString();
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                values: new object[] { userRoleId, "User", "USER", Guid.NewGuid().ToString() }
            );

            // Create admin user
            var adminUserId = Guid.NewGuid().ToString();
            var adminEmail = "admin@movieapp.com";
            
            // Create password hash for admin user
            var passwordHasher = new PasswordHasher<IdentityUser>();
            var adminUser = new IdentityUser
            {
                Id = adminUserId,
                UserName = adminEmail,
                NormalizedUserName = adminEmail.ToUpper(),
                Email = adminEmail,
                NormalizedEmail = adminEmail.ToUpper(),
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            
            var passwordHash = passwordHasher.HashPassword(adminUser, "Admin123!");
            
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] {
                    "Id", "UserName", "NormalizedUserName", "Email", "NormalizedEmail",
                    "EmailConfirmed", "PasswordHash", "SecurityStamp", "ConcurrencyStamp",
                    "PhoneNumber", "PhoneNumberConfirmed", "TwoFactorEnabled", "LockoutEnd",
                    "LockoutEnabled", "AccessFailedCount", "FirstName", "LastName", "CreatedOn", "IsActive"
                },
                values: new object[] {
                    adminUserId, adminEmail, adminEmail.ToUpper(), adminEmail, adminEmail.ToUpper(),
                    true, passwordHash, Guid.NewGuid().ToString(), Guid.NewGuid().ToString(),
                    null, false, false, null,
                    false, 0, "Admin", "User", DateTime.Now, true
                }
            );
            
            // Assign admin user to Admin role
            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" },
                values: new object[] { adminUserId, adminRoleId }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove the admin user role assignment
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "UserId", "RoleId" },
                keyValues: new object[] { null, null }
            );
            
            // Remove admin user
            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Email",
                keyValue: "admin@movieapp.com"
            );
            
            // Remove roles
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Name",
                keyValue: "Admin"
            );
            
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Name",
                keyValue: "User"
            );
        }
    }
}
