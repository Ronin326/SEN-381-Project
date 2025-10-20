using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CampusLearn_Web_App.Migrations
{
    public partial class AddForumTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create ForumPost table
            migrationBuilder.CreateTable(
                name: "forumpost",
                columns: table => new
                {
                    postid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserID = table.Column<int>(type: "integer", nullable: false),
                    StudentModuleID = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_forumpost", x => x.postid);
                    table.ForeignKey(
                        name: "FK_forumpost_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "userid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_forumpost_studentmodule_StudentModuleID",
                        column: x => x.StudentModuleID,
                        principalTable: "studentmodule",
                        principalColumn: "studentmoduleid",
                        onDelete: ReferentialAction.Cascade);
                });

            // Create ForumComment table
            migrationBuilder.CreateTable(
                name: "forumcomment",
                columns: table => new
                {
                    commentid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserID = table.Column<int>(type: "integer", nullable: false),
                    PostID = table.Column<int>(type: "integer", nullable: false),
                    ParentCommentID = table.Column<int>(type: "integer", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_forumcomment", x => x.commentid);
                    table.ForeignKey(
                        name: "FK_forumcomment_User_UserID",
                        column: x => x.UserID,
                        principalTable: "User",
                        principalColumn: "userid",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_forumcomment_forumpost_PostID",
                        column: x => x.PostID,
                        principalTable: "forumpost",
                        principalColumn: "postid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_forumcomment_forumcomment_ParentCommentID",
                        column: x => x.ParentCommentID,
                        principalTable: "forumcomment",
                        principalColumn: "commentid");
                });

            // Create indexes
            migrationBuilder.CreateIndex(
                name: "IX_forumpost_UserID",
                table: "forumpost",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_forumpost_StudentModuleID",
                table: "forumpost",
                column: "StudentModuleID");

            migrationBuilder.CreateIndex(
                name: "IX_forumcomment_UserID",
                table: "forumcomment",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_forumcomment_PostID",
                table: "forumcomment",
                column: "PostID");

            migrationBuilder.CreateIndex(
                name: "IX_forumcomment_ParentCommentID",
                table: "forumcomment",
                column: "ParentCommentID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "forumcomment");
            migrationBuilder.DropTable(name: "forumpost");
        }
    }
}
