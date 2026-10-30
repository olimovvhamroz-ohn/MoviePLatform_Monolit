using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoviePlatformMonolit.Modules.SharedKernel.Migrations
{
    /// <inheritdoc />
    public partial class AddBaseEntityTimestamps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                schema: "engagement",
                table: "watchlist",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "engagement",
                table: "watchlist",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                schema: "engagement",
                table: "watchlist",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "Views",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Views",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "Views",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "UserFavoriteCategories",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "UserFavoriteCategories",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "UserFavoriteCategories",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "Studios",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Studios",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "Studios",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                schema: "catalog",
                table: "reviews",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "catalog",
                table: "reviews",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                schema: "catalog",
                table: "reviews",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                schema: "commerce",
                table: "purchases",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "commerce",
                table: "purchases",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                schema: "commerce",
                table: "purchases",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                schema: "catalog",
                table: "movies",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "catalog",
                table: "movies",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                schema: "catalog",
                table: "movies",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "Categories",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Categories",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "Categories",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                schema: "commerce",
                table: "carts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "commerce",
                table: "carts",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                schema: "commerce",
                table: "carts",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "CartItems",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "CartItems",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "CartItems",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "Actors",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Actors",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAtUtc",
                table: "Actors",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_purchases_MovieId",
                schema: "commerce",
                table: "purchases",
                column: "MovieId");

            migrationBuilder.AddForeignKey(
                name: "FK_purchases_Users_UserId",
                schema: "commerce",
                table: "purchases",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_purchases_movies_MovieId",
                schema: "commerce",
                table: "purchases",
                column: "MovieId",
                principalSchema: "catalog",
                principalTable: "movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_purchases_Users_UserId",
                schema: "commerce",
                table: "purchases");

            migrationBuilder.DropForeignKey(
                name: "FK_purchases_movies_MovieId",
                schema: "commerce",
                table: "purchases");

            migrationBuilder.DropIndex(
                name: "IX_purchases_MovieId",
                schema: "commerce",
                table: "purchases");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                schema: "engagement",
                table: "watchlist");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "engagement",
                table: "watchlist");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUtc",
                schema: "engagement",
                table: "watchlist");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "Views");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Views");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUtc",
                table: "Views");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUtc",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "UserFavoriteCategories");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "UserFavoriteCategories");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUtc",
                table: "UserFavoriteCategories");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "Studios");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Studios");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUtc",
                table: "Studios");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                schema: "catalog",
                table: "reviews");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "catalog",
                table: "reviews");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUtc",
                schema: "catalog",
                table: "reviews");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                schema: "commerce",
                table: "purchases");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "commerce",
                table: "purchases");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUtc",
                schema: "commerce",
                table: "purchases");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                schema: "catalog",
                table: "movies");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "catalog",
                table: "movies");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUtc",
                schema: "catalog",
                table: "movies");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUtc",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                schema: "commerce",
                table: "carts");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "commerce",
                table: "carts");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUtc",
                schema: "commerce",
                table: "carts");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUtc",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "Actors");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Actors");

            migrationBuilder.DropColumn(
                name: "UpdatedAtUtc",
                table: "Actors");
        }
    }
}
