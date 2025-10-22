using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampusLearn_Web_App.Migrations
{
    /// <inheritdoc />
    public partial class AddUpvotesToForumModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Downvotes",
                table: "forumpost",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Upvotes",
                table: "forumpost",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Downvotes",
                table: "forumcomment",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Upvotes",
                table: "forumcomment",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Downvotes",
                table: "forumpost");

            migrationBuilder.DropColumn(
                name: "Upvotes",
                table: "forumpost");

            migrationBuilder.DropColumn(
                name: "Downvotes",
                table: "forumcomment");

            migrationBuilder.DropColumn(
                name: "Upvotes",
                table: "forumcomment");
        }
    }
}
