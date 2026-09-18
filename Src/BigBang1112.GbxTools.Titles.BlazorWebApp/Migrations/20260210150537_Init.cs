using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BigBang1112.GbxTools.Titles.BlazorWebApp.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Titles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DeformattedName = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(2048)", maxLength: 2048, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Punchline = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AuthorLogin = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AuthorNickname = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DownloadUrl = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreationDate = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    Cost = table.Column<int>(type: "int", nullable: false),
                    Registrations = table.Column<int>(type: "int", nullable: false),
                    PlayersLast24h = table.Column<int>(type: "int", nullable: false),
                    OnlinePlayers = table.Column<int>(type: "int", nullable: false),
                    FacebookUrl = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TwitterUrl = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    YoutubeUrl = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ForumUrl = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    WebsiteUrl = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CardUrl = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BackgroundUrl = table.Column<string>(type: "varchar(160)", maxLength: 160, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LogoUrl = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsSolo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsMultiplayer = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsEnvironment = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsTrackmania = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsShootmania = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsMatchmaking = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PrimaryColor = table.Column<string>(type: "varchar(6)", maxLength: 6, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TitleMakerUid = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TitleMakerName = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TitlePageUrl = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StoredAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    ArchivedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Titles", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "HistoricalPlayerCounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TitleId = table.Column<string>(type: "varchar(64)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Registrations = table.Column<int>(type: "int", nullable: false),
                    PlayersLast24h = table.Column<int>(type: "int", nullable: false),
                    OnlinePlayers = table.Column<int>(type: "int", nullable: false),
                    RecordedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricalPlayerCounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HistoricalPlayerCounts_Titles_TitleId",
                        column: x => x.TitleId,
                        principalTable: "Titles",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TitleMetadata",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    LastUpdate = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    ETag = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LastModified = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    ContentLength = table.Column<long>(type: "bigint", nullable: true),
                    Version = table.Column<int>(type: "int", nullable: false),
                    AuthorVersion = table.Column<int>(type: "int", nullable: true),
                    AuthorLogin = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AuthorNickname = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AuthorZone = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AuthorExtraInfo = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Flags = table.Column<uint>(type: "int unsigned", nullable: false),
                    GbxHeadersStart = table.Column<uint>(type: "int unsigned", nullable: false),
                    GbxHeadersSize = table.Column<int>(type: "int", nullable: true),
                    GbxHeadersComprSize = table.Column<int>(type: "int", nullable: true),
                    HeaderMaxSize = table.Column<int>(type: "int", nullable: true),
                    Size = table.Column<uint>(type: "int unsigned", nullable: true),
                    HeaderMD5 = table.Column<byte[]>(type: "varbinary(16)", maxLength: 16, nullable: true),
                    Checksum = table.Column<byte[]>(type: "varbinary(32)", maxLength: 32, nullable: true),
                    HeaderFlags = table.Column<uint>(type: "int unsigned", nullable: true),
                    Comments = table.Column<string>(type: "varchar(1024)", maxLength: 1024, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreationBuildInfo = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AuthorUrl = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ManialinkUrl = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DownloadUrl = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreationDate = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: true),
                    Xml = table.Column<string>(type: "varchar(1024)", maxLength: 1024, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TitleId = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UsageSubDir = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    U01 = table.Column<byte[]>(type: "varbinary(16)", maxLength: 16, nullable: true),
                    StoredAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TitleMetadata", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TitleMetadata_Titles_TitleId",
                        column: x => x.TitleId,
                        principalTable: "Titles",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TitleScripts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TitleId = table.Column<string>(type: "varchar(64)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FileName = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MatchSettings = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TitleScripts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TitleScripts_Titles_TitleId",
                        column: x => x.TitleId,
                        principalTable: "Titles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TitleFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TitleMetadataId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FolderPath = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClassId = table.Column<uint>(type: "int unsigned", nullable: false),
                    Offset = table.Column<uint>(type: "int unsigned", nullable: false),
                    UncompressedSize = table.Column<int>(type: "int", nullable: false),
                    CompressedSize = table.Column<int>(type: "int", nullable: false),
                    Size = table.Column<int>(type: "int", nullable: true),
                    Checksum = table.Column<byte[]>(type: "varbinary(16)", maxLength: 16, nullable: true),
                    Flags = table.Column<ulong>(type: "bigint unsigned", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TitleFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TitleFiles_TitleMetadata_TitleMetadataId",
                        column: x => x.TitleMetadataId,
                        principalTable: "TitleMetadata",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TitleIncludedPacks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TitleMetadataId = table.Column<int>(type: "int", nullable: false),
                    ContentsChecksum = table.Column<byte[]>(type: "varbinary(32)", maxLength: 32, nullable: false),
                    Name = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AuthorVersion = table.Column<int>(type: "int", nullable: false),
                    AuthorLogin = table.Column<string>(type: "varchar(32)", maxLength: 32, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AuthorNickname = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AuthorZone = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AuthorExtraInfo = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    InfoManialinkUrl = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreationDate = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false),
                    IncludeDepth = table.Column<uint>(type: "int unsigned", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TitleIncludedPacks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TitleIncludedPacks_TitleMetadata_TitleMetadataId",
                        column: x => x.TitleMetadataId,
                        principalTable: "TitleMetadata",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricalPlayerCounts_TitleId",
                table: "HistoricalPlayerCounts",
                column: "TitleId");

            migrationBuilder.CreateIndex(
                name: "IX_TitleFiles_TitleMetadataId",
                table: "TitleFiles",
                column: "TitleMetadataId");

            migrationBuilder.CreateIndex(
                name: "IX_TitleIncludedPacks_TitleMetadataId",
                table: "TitleIncludedPacks",
                column: "TitleMetadataId");

            migrationBuilder.CreateIndex(
                name: "IX_TitleMetadata_TitleId",
                table: "TitleMetadata",
                column: "TitleId");

            migrationBuilder.CreateIndex(
                name: "IX_TitleScripts_TitleId",
                table: "TitleScripts",
                column: "TitleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistoricalPlayerCounts");

            migrationBuilder.DropTable(
                name: "TitleFiles");

            migrationBuilder.DropTable(
                name: "TitleIncludedPacks");

            migrationBuilder.DropTable(
                name: "TitleScripts");

            migrationBuilder.DropTable(
                name: "TitleMetadata");

            migrationBuilder.DropTable(
                name: "Titles");
        }
    }
}
