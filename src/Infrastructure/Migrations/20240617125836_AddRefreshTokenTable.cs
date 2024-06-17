using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshTokenTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "refresh_token",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Token = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserInfoId = table.Column<int>(type: "integer", nullable: false),
                    UserInfoId1 = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refresh_token", x => x.Id);
                    table.ForeignKey(
                        name: "FK_refresh_token_user_info_UserInfoId",
                        column: x => x.UserInfoId,
                        principalTable: "user_info",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_refresh_token_user_info_UserInfoId1",
                        column: x => x.UserInfoId1,
                        principalTable: "user_info",
                        principalColumn: "id");
                });


            migrationBuilder.CreateIndex(
                name: "IX_refresh_token_ExpiresAt",
                table: "refresh_token",
                column: "ExpiresAt");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_token_Token",
                table: "refresh_token",
                column: "Token");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_token_UserInfoId",
                table: "refresh_token",
                column: "UserInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_refresh_token_UserInfoId1",
                table: "refresh_token",
                column: "UserInfoId1");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropTable(
                name: "refresh_token");
        }
    }
}
