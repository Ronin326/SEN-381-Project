using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CampusLearn_Web_App.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "module",
                columns: table => new
                {
                    moduleid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    modulename = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    modulecode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_module", x => x.moduleid);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    userid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    firstname = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    lastname = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    passwordhash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    role = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.userid);
                });

            migrationBuilder.CreateTable(
                name: "Message",
                columns: table => new
                {
                    messageid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    senderid = table.Column<int>(type: "integer", nullable: false),
                    receiverid = table.Column<int>(type: "integer", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    sentdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Message", x => x.messageid);
                    table.ForeignKey(
                        name: "FK_Message_User_receiverid",
                        column: x => x.receiverid,
                        principalTable: "User",
                        principalColumn: "userid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Message_User_senderid",
                        column: x => x.senderid,
                        principalTable: "User",
                        principalColumn: "userid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    notificationid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    message = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    createdat = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    userid = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.notificationid);
                    table.ForeignKey(
                        name: "FK_Notification_User_userid",
                        column: x => x.userid,
                        principalTable: "User",
                        principalColumn: "userid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "studentmodule",
                columns: table => new
                {
                    studentmoduleid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    userid = table.Column<int>(type: "integer", nullable: false),
                    moduleid = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_studentmodule", x => x.studentmoduleid);
                    table.ForeignKey(
                        name: "FK_studentmodule_User_userid",
                        column: x => x.userid,
                        principalTable: "User",
                        principalColumn: "userid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_studentmodule_module_moduleid",
                        column: x => x.moduleid,
                        principalTable: "module",
                        principalColumn: "moduleid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Topic",
                columns: table => new
                {
                    topicid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    creationdate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    userid = table.Column<int>(type: "integer", nullable: false),
                    moduleid = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Topic", x => x.topicid);
                    table.ForeignKey(
                        name: "FK_Topic_User_userid",
                        column: x => x.userid,
                        principalTable: "User",
                        principalColumn: "userid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Topic_module_moduleid",
                        column: x => x.moduleid,
                        principalTable: "module",
                        principalColumn: "moduleid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "tutormodule",
                columns: table => new
                {
                    tutormoduleid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    userid = table.Column<int>(type: "integer", nullable: false),
                    moduleid = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tutormodule", x => x.tutormoduleid);
                    table.ForeignKey(
                        name: "FK_tutormodule_User_userid",
                        column: x => x.userid,
                        principalTable: "User",
                        principalColumn: "userid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_tutormodule_module_moduleid",
                        column: x => x.moduleid,
                        principalTable: "module",
                        principalColumn: "moduleid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LearningMaterial",
                columns: table => new
                {
                    materialid = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    filename = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    filetype = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    uploaddate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    topicid = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LearningMaterial", x => x.materialid);
                    table.ForeignKey(
                        name: "FK_LearningMaterial_Topic_topicid",
                        column: x => x.topicid,
                        principalTable: "Topic",
                        principalColumn: "topicid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LearningMaterial_topicid",
                table: "LearningMaterial",
                column: "topicid");

            migrationBuilder.CreateIndex(
                name: "IX_Message_receiverid",
                table: "Message",
                column: "receiverid");

            migrationBuilder.CreateIndex(
                name: "IX_Message_senderid",
                table: "Message",
                column: "senderid");

            migrationBuilder.CreateIndex(
                name: "IX_module_modulecode",
                table: "module",
                column: "modulecode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notification_userid",
                table: "Notification",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "IX_studentmodule_moduleid",
                table: "studentmodule",
                column: "moduleid");

            migrationBuilder.CreateIndex(
                name: "IX_studentmodule_userid",
                table: "studentmodule",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "IX_Topic_moduleid",
                table: "Topic",
                column: "moduleid");

            migrationBuilder.CreateIndex(
                name: "IX_Topic_userid",
                table: "Topic",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "IX_tutormodule_moduleid",
                table: "tutormodule",
                column: "moduleid");

            migrationBuilder.CreateIndex(
                name: "IX_tutormodule_userid",
                table: "tutormodule",
                column: "userid");

            migrationBuilder.CreateIndex(
                name: "IX_User_email",
                table: "User",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LearningMaterial");

            migrationBuilder.DropTable(
                name: "Message");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "studentmodule");

            migrationBuilder.DropTable(
                name: "tutormodule");

            migrationBuilder.DropTable(
                name: "Topic");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "module");
        }
    }
}
