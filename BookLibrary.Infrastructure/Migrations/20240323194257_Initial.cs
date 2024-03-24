using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookLibrary.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "book_library");

            migrationBuilder.CreateTable(
                name: "abonents",
                schema: "book_library",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    surname = table.Column<string>(type: "text", nullable: false),
                    patronymic = table.Column<string>(type: "text", nullable: true),
                    email = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_abonents", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "books",
                schema: "book_library",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "text", nullable: false),
                    isbn = table.Column<string>(type: "text", nullable: false),
                    publication_date = table.Column<DateOnly>(type: "date", nullable: false),
                    authors = table.Column<string>(type: "jsonb", nullable: false),
                    borrowed_by_abonent_id = table.Column<Guid>(type: "uuid", nullable: true),
                    borrowed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    borrowed_return_before = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_books", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_abonents_email",
                schema: "book_library",
                table: "abonents",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_books_isbn_publication_date",
                schema: "book_library",
                table: "books",
                columns: new[] { "isbn", "publication_date" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "abonents",
                schema: "book_library");

            migrationBuilder.DropTable(
                name: "books",
                schema: "book_library");
        }
    }
}
