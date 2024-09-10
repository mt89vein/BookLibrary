using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace BookLibrary.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBookStat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:pg_trgm", ",,");

            migrationBuilder.CreateTable(
                name: "book_stat_changes",
                schema: "book_library",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    isbn = table.Column<string>(type: "text", nullable: false),
                    publication_date = table.Column<DateOnly>(type: "date", nullable: false),
                    available_count = table.Column<int>(type: "integer", nullable: false),
                    borrowed_count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_book_stat_changes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "book_stats",
                schema: "book_library",
                columns: table => new
                {
                    isbn = table.Column<string>(type: "text", nullable: false),
                    publication_date = table.Column<DateOnly>(type: "date", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    authors = table.Column<string>(type: "text", nullable: false),
                    available_count = table.Column<int>(type: "integer", nullable: false),
                    borrowed_count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_book_stats", x => new { x.isbn, x.publication_date });
                });

            migrationBuilder.CreateIndex(
                name: "ix_book_stats_authors",
                schema: "book_library",
                table: "book_stats",
                column: "authors")
                .Annotation("Npgsql:IndexMethod", "gin")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });

            migrationBuilder.CreateIndex(
                name: "ix_book_stats_title",
                schema: "book_library",
                table: "book_stats",
                column: "title")
                .Annotation("Npgsql:IndexMethod", "gin")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "book_stat_changes",
                schema: "book_library");

            migrationBuilder.DropTable(
                name: "book_stats",
                schema: "book_library");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:PostgresExtension:pg_trgm", ",,");
        }
    }
}