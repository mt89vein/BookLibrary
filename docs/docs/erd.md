---
title: Entity relationship diagram
---

```kroki type=dbml
Table "book_library"."abonents" {
  "id" uuid [pk, not null]
  "name" text [not null]
  "surname" text [not null]
  "patronymic" text
  "email" text [unique, not null]
  "created_at" timestamptz [not null, default: '-infinity']
}

Table "book_library"."books" {
  "id" uuid [pk, not null]
  "title" text [not null]
  "isbn" text [not null]
  "publication_date" date [not null]
  "authors" jsonb [not null]
  "borrowed_by_abonent_id" uuid
  "borrowed_at" timestamptz
  "borrowed_return_before" date
  "created_at" timestamptz [not null, default: '-infinity']

  Indexes {
    (isbn, publication_date) [type: btree, name: "ix_books_isbn_publication_date"]
  }
}

Table "book_library"."book_stat_changes" {
  "id" uuid [pk, not null]
  "isbn" text [not null]
  "publication_date" date [not null]
  "available_count" int4 [not null]
  "borrowed_count" int4 [not null]
}

Table "book_library"."book_stats" {
  "isbn" text [not null]
  "publication_date" date [not null]
  "title" text [not null]
  "authors" text [not null]
  "available_count" int4 [not null]
  "borrowed_count" int4 [not null]

  Indexes {
    (isbn, publication_date) [type: btree, name: "pk_book_stats"]
    authors [type: gin, name: "ix_book_stats_authors"]
    title [type: gin, name: "ix_book_stats_title"]
  }
}
```