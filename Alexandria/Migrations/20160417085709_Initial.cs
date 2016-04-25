using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;
using Microsoft.Data.Entity.Metadata;

namespace Alexandria.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Library",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Enabled = table.Column<bool>(nullable: false),
                    Expire = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    Path = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Library", x => x.ID);
                });
            migrationBuilder.CreateTable(
                name: "TagCategory",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagCategory", x => x.ID);
                });
            migrationBuilder.CreateTable(
                name: "UserAccount",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Admin = table.Column<bool>(nullable: false),
                    Email = table.Column<string>(nullable: false),
                    Password = table.Column<string>(nullable: true),
                    System = table.Column<bool>(nullable: false),
                    UserName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAccount", x => x.ID);
                });
            migrationBuilder.CreateTable(
                name: "File",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Archival = table.Column<bool>(nullable: false),
                    FileModified = table.Column<DateTime>(nullable: false),
                    LibraryID = table.Column<int>(nullable: false),
                    Path = table.Column<string>(nullable: false),
                    RecordAdded = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_File", x => x.ID);
                    table.ForeignKey(
                        name: "FK_File_Library_LibraryID",
                        column: x => x.LibraryID,
                        principalTable: "Library",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateTable(
                name: "Tag",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CategoryID = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tag", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Tag_TagCategory_CategoryID",
                        column: x => x.CategoryID,
                        principalTable: "TagCategory",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateTable(
                name: "VirtualAlbum",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FileID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VirtualAlbum", x => x.ID);
                    table.ForeignKey(
                        name: "FK_VirtualAlbum_File_FileID",
                        column: x => x.FileID,
                        principalTable: "File",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });
            migrationBuilder.CreateTable(
                name: "VirtualTrack",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FileID = table.Column<int>(nullable: true),
                    vAlbumID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VirtualTrack", x => x.ID);
                    table.ForeignKey(
                        name: "FK_VirtualTrack_File_FileID",
                        column: x => x.FileID,
                        principalTable: "File",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VirtualTrack_VirtualAlbum_vAlbumID",
                        column: x => x.vAlbumID,
                        principalTable: "VirtualAlbum",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });
            migrationBuilder.CreateTable(
                name: "TagAssociation",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FileID = table.Column<int>(nullable: true),
                    TagID = table.Column<int>(nullable: false),
                    VirtualAlbumID = table.Column<int>(nullable: true),
                    VirtualTrackID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagAssociation", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TagAssociation_File_FileID",
                        column: x => x.FileID,
                        principalTable: "File",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TagAssociation_Tag_TagID",
                        column: x => x.TagID,
                        principalTable: "Tag",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TagAssociation_VirtualAlbum_VirtualAlbumID",
                        column: x => x.VirtualAlbumID,
                        principalTable: "VirtualAlbum",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TagAssociation_VirtualTrack_VirtualTrackID",
                        column: x => x.VirtualTrackID,
                        principalTable: "VirtualTrack",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });
            migrationBuilder.CreateTable(
                name: "TagAdder",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TagAssociationID = table.Column<int>(nullable: true),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    UserID = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagAdder", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TagAdder_TagAssociation_TagAssociationID",
                        column: x => x.TagAssociationID,
                        principalTable: "TagAssociation",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TagAdder_UserAccount_UserID",
                        column: x => x.UserID,
                        principalTable: "UserAccount",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("TagAdder");
            migrationBuilder.DropTable("TagAssociation");
            migrationBuilder.DropTable("UserAccount");
            migrationBuilder.DropTable("Tag");
            migrationBuilder.DropTable("VirtualTrack");
            migrationBuilder.DropTable("TagCategory");
            migrationBuilder.DropTable("VirtualAlbum");
            migrationBuilder.DropTable("File");
            migrationBuilder.DropTable("Library");
        }
    }
}
